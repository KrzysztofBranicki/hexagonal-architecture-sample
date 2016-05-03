using System;
using System.Collections.Generic;

namespace Common.Domain.Tests.Events
{
    class ObjectsCreator
    {
        public Func<Type, object> InstanceCreatorFunction { get; }
        public IReadOnlyCollection<object> CreatedInstances { get; }

        private ObjectsCreator(Func<Type, object> instanceCreatorFunction, List<object> createdInstances)
        {
            InstanceCreatorFunction = instanceCreatorFunction;
            CreatedInstances = createdInstances;
        }

        public static ObjectsCreator CreateNew(Func<Type, object> instanceCreatorFunction = null)
        {
            instanceCreatorFunction = instanceCreatorFunction ?? Activator.CreateInstance;

            var createdObjects = new List<object>();
            Func<Type, object> creatorFunction = type =>
            {
                var instance = instanceCreatorFunction(type);
                createdObjects.Add(instance);
                return instance;
            };

            return new ObjectsCreator(creatorFunction, createdObjects);
        }
    }
}
