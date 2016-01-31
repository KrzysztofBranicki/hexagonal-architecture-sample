using System;

namespace Common.Domain.Repositories.Exceptions
{
    [Serializable]
    public class EntityDeleteFailedException : Exception
    {
        public object Id { get; }

        public EntityDeleteFailedException(object id)
            : base($"Entity id: {id} delete failed")
        {
            Id = id;
        }
    }
}