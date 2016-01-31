using System;

namespace Common.Domain.Repositories.Exceptions
{
    [Serializable]
    public class EntityUpdateFailedException : Exception
    {
        public object Id { get; }

        public EntityUpdateFailedException(object id)
            : base($"Entity id: {id} update failed")
        {
            Id = id;
        }
    }
}