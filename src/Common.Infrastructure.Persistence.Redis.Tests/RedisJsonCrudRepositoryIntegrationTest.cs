using System;
using System.Configuration;
using Common.Domain.Repositories;
using Common.Domain.Tests.Repositories;
using Common.Infrastructure.Persistence.Redis.Repositories;
using NUnit.Framework;
using StackExchange.Redis;

namespace Common.Infrastructure.Persistence.Redis.Tests
{
    [TestFixture]
    public class RedisJsonCrudRepositoryIntegrationTest : GenericCrudRepositoryTest
    {
        private string _redisServerAddress;
        private ConnectionMultiplexer _connectionMultiplexer;

        [OneTimeSetUp]
        public void Setup()
        {
            _redisServerAddress = ConfigurationManager.AppSettings["RedisServerAddress"];
            _connectionMultiplexer = ConnectionMultiplexer.Connect(_redisServerAddress + ",allowAdmin=true");
        }

        protected override ICrudRepository<TestEntity, Guid> CreateRepository()
        {
            _connectionMultiplexer.GetServer(_redisServerAddress).FlushDatabase();
            return new RedisJsonCrudRepository<TestEntity, Guid>(_connectionMultiplexer);
        }
    }
}
