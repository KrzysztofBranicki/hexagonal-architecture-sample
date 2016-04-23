using System;
using System.Configuration;
using Common.Domain.Events;
using Common.Domain.Tests.Events;
using Common.Logging;
using NUnit.Framework;
using StackExchange.Redis;

namespace Common.Infrastructure.Messaging.Redis.Tests
{
    [TestFixture]
    public class RedisEventBrokerIntegrationTest : GenericEventBrokerTest
    {
        private ConnectionMultiplexer _connectionMultiplexer;

        [OneTimeSetUp]
        public void Setup()
        {
            _connectionMultiplexer = ConnectionMultiplexer.Connect(ConfigurationManager.AppSettings["RedisServerAddress"]);
        }

        protected override IEventBroker GetEventBroker(Func<Type, object> handlerInstanceCreator = null)
        {
            _connectionMultiplexer.GetSubscriber().UnsubscribeAll();
            return new EventBrokerDelayAfterPublishDecorator(new RedisEventBroker(_connectionMultiplexer, NullLogger.Instance), TimeSpan.FromSeconds(0.5));
        }
    }
}
