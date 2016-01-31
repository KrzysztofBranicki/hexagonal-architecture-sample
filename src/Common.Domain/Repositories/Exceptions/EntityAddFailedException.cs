using System;

namespace Common.Domain.Repositories.Exceptions
{
    [Serializable]
    public class EntityAddFailedException : Exception
    {
        public object Id { get; }

        public EntityAddFailedException(object id)
            : base($"Entity id: {id} add failed")
        {
            Id = id;
        }
    }
}
