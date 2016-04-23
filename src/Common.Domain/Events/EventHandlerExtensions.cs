using System;
using System.Collections.Generic;
using System.Linq;

namespace Common.Domain.Events
{
    public static class EventHandlerExtensions
    {
        public static IEnumerable<Type> GetEventTypesWhichHandlerSupports<TEvent>(this IEventHandler<TEvent> eventHandler) where TEvent : class
        {
            return GetEventTypesWhichHandlerSupports(eventHandler.GetType());
        }

        public static IEnumerable<Type> GetEventTypesWhichHandlerSupports<TEventHandler>() where TEventHandler : IEventHandler
        {
            return GetEventTypesWhichHandlerSupports(typeof(TEventHandler));
        }

        public static IEnumerable<Type> GetEventTypesWhichHandlerSupports(Type eventHandlerType)
        {
            var handledEventNames = eventHandlerType
                .GetInterfaces()
                .Union(eventHandlerType.IsInterface ? new[] { eventHandlerType } : Enumerable.Empty<Type>())
                .Where(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEventHandler<>))
                .Select(x => x.GetGenericArguments().Single())
                .ToList();

            return handledEventNames;
        }
    }
}
