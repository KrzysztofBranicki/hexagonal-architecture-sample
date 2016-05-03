using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Common.Domain.Events;
using Common.Logging;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;

namespace Common.Infrastructure.Messaging.AzureServiceBus
{
    public class ServiceBusTopicEventBroker : IEventBroker
    {
        private readonly string _connectionString;
        private readonly ILogger _logger;
        private readonly bool _rethrowEventHandlerExceptions;

        private readonly ConcurrentDictionary<object, List<SubscriptionClient>> _subscriptionClientsForEventHandler = new ConcurrentDictionary<object, List<SubscriptionClient>>();

        public ServiceBusTopicEventBroker(string connectionString, ILogger logger, bool rethrowEventHandlerExceptions = true)
        {
            _connectionString = connectionString;
            _logger = logger;
            _rethrowEventHandlerExceptions = rethrowEventHandlerExceptions;
        }

        public void Publish<T>(T eventObject) where T : class
        {
            var topicName = GetTopicNameForEventType(eventObject.GetType());
            CreateTopicIfNotExists(topicName);
            var topicClient = TopicClient.CreateFromConnectionString(_connectionString, topicName);
            topicClient.Send(new BrokeredMessage(SerializeEventObject(eventObject)));
        }

        public void SubscribeHandlerInstance<TEvent>(IEventHandler<TEvent> eventHandler) where TEvent : class
        {
            var subscriptionClients = new List<SubscriptionClient>();
            if (_subscriptionClientsForEventHandler.TryAdd(eventHandler, subscriptionClients))
            {
                foreach (var handledEventType in eventHandler.GetEventTypesWhichHandlerSupports())
                {
                    string topicName = GetTopicNameForEventType(handledEventType);
                    var subscriptionName = GetSubscriptionNameForEventHandler(eventHandler);
                    SubscribeToTopicIfNotAlreadySubscribed(topicName, subscriptionName);

                    var subscriptionClient = SubscriptionClient.CreateFromConnectionString(_connectionString, topicName, subscriptionName);

                    var options = new OnMessageOptions
                    {
                        AutoComplete = false,
                        AutoRenewTimeout = TimeSpan.FromMinutes(1),
                    };

                    subscriptionClient.OnMessage((message) =>
                    {
                        try
                        {
                            var eventObject = DeserializeEventObject(message.GetBody<string>(), handledEventType);
                            dynamic dynamicHandler = eventHandler;
                            dynamicHandler.Handle(eventObject);

                            message.Complete();
                        }
                        catch (Exception e)
                        {
                            message.Abandon();
                            _logger.Error(e);
                            if (_rethrowEventHandlerExceptions)
                                throw;
                        }
                    }, options);

                    subscriptionClients.Add(subscriptionClient);
                }
            }
        }

        public void UnsubscribeHandlerInstance<TEvent>(IEventHandler<TEvent> eventHandler) where TEvent : class
        {
            List<SubscriptionClient> subscriptionClients;
            if (_subscriptionClientsForEventHandler.TryRemove(eventHandler, out subscriptionClients))
            {
                var namespaceManager = NamespaceManager.CreateFromConnectionString(_connectionString);
                foreach (var handledEventType in eventHandler.GetEventTypesWhichHandlerSupports())
                {
                    string topicName = GetTopicNameForEventType(handledEventType);
                    var subscriptionName = GetSubscriptionNameForEventHandler(eventHandler);

                    namespaceManager.DeleteSubscription(topicName, subscriptionName);
                }
            }
        }

        public void SubscribeHandlerType<TEventHandler>() where TEventHandler : IEventHandler
        {
            throw new NotImplementedException();
        }

        private void CreateTopicIfNotExists(string topicName)
        {
            var namespaceManager = NamespaceManager.CreateFromConnectionString(_connectionString);
            if (!namespaceManager.TopicExists(topicName))
            {
                namespaceManager.CreateTopic(topicName);
            }
        }

        private void SubscribeToTopicIfNotAlreadySubscribed(string topicName, string subscriptionName)
        {
            CreateTopicIfNotExists(topicName);
            var namespaceManager = NamespaceManager.CreateFromConnectionString(_connectionString);
            if (!namespaceManager.SubscriptionExists(topicName, subscriptionName))
            {
                namespaceManager.CreateSubscription(topicName, subscriptionName);
            }
        }

        protected virtual string GetTopicNameForEventType(Type eventType)
        {
            return ReplaceServiceBusRestrictedCharacters(eventType.FullName);
        }

        protected virtual string GetSubscriptionNameForEventHandler<TEvent>(IEventHandler<TEvent> eventHandler) where TEvent : class
        {
            return ReplaceServiceBusRestrictedCharacters(eventHandler.GetType().FullName);
        }

        protected virtual string ReplaceServiceBusRestrictedCharacters(string name)
        {
            return name.Replace('+', '-').Replace('`', '_');
        }

        protected virtual string SerializeEventObject(object eventObject)
        {
            return JsonConvert.SerializeObject(eventObject);
        }

        protected virtual dynamic DeserializeEventObject(string jsonEvent, Type eventType)
        {
            return JsonConvert.DeserializeObject(jsonEvent, eventType);
        }
    }
}
