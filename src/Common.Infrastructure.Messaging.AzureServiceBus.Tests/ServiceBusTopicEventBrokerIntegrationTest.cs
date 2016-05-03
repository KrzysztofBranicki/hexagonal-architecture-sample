using System;
using System.Configuration;
using Common.Domain.Events;
using Common.Domain.Tests.Events;
using Common.Logging;
using Microsoft.ServiceBus;
using NUnit.Framework;

namespace Common.Infrastructure.Messaging.AzureServiceBus.Tests
{

    [TestFixture]
    [Parallelizable(ParallelScope.None)]
    public class ServiceBusTopicEventBrokerIntegrationTest : GenericEventBrokerTest
    {
        private string _connectionString;

        [OneTimeSetUp]
        public void Setup()
        {
            _connectionString = ConfigurationManager.AppSettings["ServiceBusConnectionString"];
            DeleteAllTopics();
        }

        protected override IEventBroker GetEventBroker(Func<Type, object> handlerInstanceCreator = null)
        {
            return new EventBrokerDelayAfterPublishDecorator(new ObjectInstanceTopicNamingServiceBusTopicEventBroker(_connectionString), TimeSpan.FromSeconds(3));
        }

        [TearDown]
        public void CleanTopics()
        {
            //unfortunately there is no api at the moment for deleting all topics from Service Bus namespace. Because of that we need to delete them one by one explicitly specifying the name.
            //This far from ideal because we need to know what convention ServiceBusTopicEventBroker is using to name them but this will have to work for now
            DeleteAllTopics("Common.Domain.Tests.Events-BaseTestEvent",
                            "Common.Domain.Tests.Events-EventA",
                            "Common.Domain.Tests.Events-EventB");
        }

        private void DeleteAllTopics(params string[] topicNames)
        {
            var namespaceManager = NamespaceManager.CreateFromConnectionString(_connectionString);
            foreach (var topicName in topicNames)
            {
                if (!namespaceManager.TopicExists(topicName))
                {
                    namespaceManager.DeleteTopic(topicName);
                }
            }
        }

        public class ObjectInstanceTopicNamingServiceBusTopicEventBroker : ServiceBusTopicEventBroker
        {
            public ObjectInstanceTopicNamingServiceBusTopicEventBroker(string connectionString) : base(connectionString, NullLogger.Instance)
            { }

            protected override string GetSubscriptionNameForEventHandler<TEvent>(IEventHandler<TEvent> eventHandler)
            {
                return ReplaceServiceBusRestrictedCharacters(base.GetSubscriptionNameForEventHandler(eventHandler) + eventHandler.GetHashCode());
            }
        }
    }
}
