using System;
using Common.Domain.Repositories;
using Common.Domain.Repositories.InMemory;
using NUnit.Framework;

namespace Common.Domain.Tests.Repositories
{
    [TestFixture]
    public class InMemoryRepositoryTest : GenericRepositoryTest
    {
        protected override IRepository<TestEntity, Guid> CreateRepository()
        {
            return new InMemoryRepository<TestEntity, Guid>();
        }
    }
}
