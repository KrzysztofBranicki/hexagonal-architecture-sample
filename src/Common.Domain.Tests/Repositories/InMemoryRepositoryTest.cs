using System;
using Common.Domain.Repositories;
using NUnit.Framework;

namespace Common.Domain.Tests.Repositories
{
    [TestFixture]
    public class InMemoryRepositoryTest
    {
        private static readonly IRepository<TestEntity, Guid> repository = new InMemoryRepository<TestEntity, Guid>();

        [Test]
        public void Get_should_throw_entity_not_found_exception_when_thers_no_entity_with_specified_id()
        {
            var missingEntityId = Guid.NewGuid();
            EntityNotFountException exception = null;

            try
            {
                repository.Get(missingEntityId);
            }
            catch (EntityNotFountException e)
            {
                exception = e;
            }

            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Id, Is.EqualTo(missingEntityId));
        }

        [Test]
        public void Get_entity_or_default_should_return_null_when_thers_no_entity_with_specified_id()
        {
            var missingEntityId = Guid.NewGuid();
            var entity = repository.GetEntityOrDefault(missingEntityId);
            Assert.That(entity, Is.Null);
        }

        [Test]
        public void Added_entity_should_be_retrievable()
        {
            var id = Guid.NewGuid();
            var testvalue = "TestValue";

            repository.Add(new TestEntity { Id = id, Value = testvalue });
            var retrievedEntity = repository.Get(id);

            Assert.That(retrievedEntity.Id, Is.EqualTo(id));
            Assert.That(retrievedEntity.Value, Is.EqualTo(testvalue));
        }

        [Test]
        public void Deleted_entity_should_be_removed_form_repository()
        {
            var id = Guid.NewGuid();

            repository.Add(new TestEntity { Id = id, Value = "testValue" });
            var retrievedEntity = repository.Get(id);

            Assert.That(retrievedEntity, Is.Not.Null);

            repository.Delete(id);

            Assert.That(() => repository.Get(id), Throws.InstanceOf<EntityNotFountException>());
        }

        [Test]
        public void Updated_entity_should_have_updated_values()
        {
            var id = Guid.NewGuid();

            repository.Add(new TestEntity { Id = id, Value = "testValue" });
            var newValue = "newValue";
            var newVersionOfEntity = new TestEntity { Id = id, Value = newValue };
            repository.Update(newVersionOfEntity);
            var retrievedEntity = repository.Get(id);

            Assert.That(retrievedEntity.Value, Is.EqualTo(newValue));
        }

        class TestEntity : IEntity<Guid>
        {
            public Guid Id { get; set; }
            public string Value { get; set; }
        }
    }
}
