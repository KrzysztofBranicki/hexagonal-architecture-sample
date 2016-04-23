using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Common.Domain.Events;
using Common.Logging;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Common.Infrastructure.Messaging.Redis
{
    public class RedisEventBroker : IEventBroker
    {
        private readonly ConnectionMultiplexer _connectionMultiplexer;
        private readonly ILogger _logger;
        private readonly bool _rethrowEventHandlerExceptions;

        private readonly ConcurrentDictionary<string, EventHandlersCollection> _handlersForChannel = new ConcurrentDictionary<string, EventHandlersCollection>();

        public RedisEventBroker(ConnectionMultiplexer connectionMultiplexer, ILogger logger, bool rethrowEventHandlerExceptions = false)
        {
            _connectionMultiplexer = connectionMultiplexer;
            _logger = logger;
            _rethrowEventHandlerExceptions = rethrowEventHandlerExceptions;
        }

        public void Publish<T>(T eventObject) where T : class
        {
            var channel = GetChannelNameForEventType(eventObject.GetType());
            var message = SerializeEventObject(eventObject);

            _connectionMultiplexer.GetSubscriber().Publish(channel, message);
        }

        public void SubscribeHandlerInstance<TEvent>(IEventHandler<TEvent> eventHandler) where TEvent : class
        {
            foreach (var handledEventType in eventHandler.GetEventTypesWhichHandlerSupports())
            {
                var channel = GetChannelNameForEventType(handledEventType);

                var isChannelAlreadySubscribed = true;

                var handlers = _handlersForChannel.GetOrAdd(channel, s =>
                {
                    isChannelAlreadySubscribed = false;
                    return new EventHandlersCollection(handledEventType);
                });

                handlers.AddEventHandler(eventHandler);

                if (!isChannelAlreadySubscribed)
                {
                    _connectionMultiplexer.GetSubscriber().Subscribe(channel, (redisChannel, value) =>
                    {
                        EventHandlersCollection registeredHandlers;
                        if (_handlersForChannel.TryGetValue(channel, out registeredHandlers))
                        {
                            foreach (dynamic registeredHandler in registeredHandlers.EventHandlers)
                            {
                                try
                                {
                                    var deserializedObject = DeserializeEventObject(value, registeredHandlers.EventType);
                                    registeredHandler.Handle(deserializedObject);
                                }
                                catch (Exception e)
                                {
                                    _logger.Error(e);
                                    if (_rethrowEventHandlerExceptions)
                                        throw;
                                }
                            }
                        }
                    });
                }

            }
        }

        public void UnsubscribeHandlerInstance<TEvent>(IEventHandler<TEvent> eventHandler) where TEvent : class
        {
            foreach (var handledEventType in eventHandler.GetEventTypesWhichHandlerSupports())
            {
                var channel = GetChannelNameForEventType(handledEventType);

                EventHandlersCollection registeredHandlers;
                if (_handlersForChannel.TryGetValue(channel, out registeredHandlers))
                {
                    registeredHandlers.RemoveEventHandler(eventHandler);
                }
            }
        }

        public void SubscribeHandlerType<TEventHandler>() where TEventHandler : IEventHandler
        {
            throw new NotImplementedException();
        }

        protected virtual string GetChannelNameForEventType(Type eventType)
        {
            return eventType.FullName;
        }

        protected virtual string SerializeEventObject(object eventObject)
        {
            return JsonConvert.SerializeObject(eventObject);
        }

        protected virtual dynamic DeserializeEventObject(string jsonEvent, Type eventType)
        {
            return JsonConvert.DeserializeObject(jsonEvent, eventType);
        }


        class EventHandlersCollection
        {
            public Type EventType { get; }
            public IEnumerable<object> EventHandlers => _eventHandlers;

            private HashSet<object> _eventHandlers = new HashSet<object>();
            private readonly object _sync = new object();

            public EventHandlersCollection(Type eventType)
            {
                EventType = eventType;
            }

            public void AddEventHandler(object eventHandler)
            {
                lock (_sync)
                {
                    var newHandlers = new HashSet<object>(_eventHandlers) { eventHandler };
                    _eventHandlers = newHandlers;
                }
            }

            public void RemoveEventHandler(object eventHandler)
            {
                lock (_sync)
                {
                    var newHandlers = new HashSet<object>(_eventHandlers);
                    newHandlers.Remove(eventHandler);
                    _eventHandlers = newHandlers;
                }
            }
        }

    }
}
