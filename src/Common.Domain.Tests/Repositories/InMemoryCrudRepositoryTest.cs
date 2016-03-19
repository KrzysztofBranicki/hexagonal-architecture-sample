using System;
using Common.Domain.Repositories;
using Common.Domain.Repositories.InMemory;
using NUnit.Framework;

namespace Common.Domain.Tests.Repositories
{
    [TestFixture]
    public class InMemoryCrudRepositoryTest : GenericCrudRepositoryTest
    {
        protected override ICrudRepository<TestEntity, Guid> CreateRepository()
        {
            return new InMemoryCrudRepository<TestEntity, Guid>();
        }
    }
}
