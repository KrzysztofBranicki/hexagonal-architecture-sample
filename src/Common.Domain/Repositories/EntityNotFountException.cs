using System;

namespace Common.Domain.Repositories
{
    [Serializable]
    public class EntityNotFountException : Exception
    {
        public object Id { get; }

        public EntityNotFountException(object id)
            : base($"Entity id: {id} not found")
        {
            Id = id;
        }
    }
}
