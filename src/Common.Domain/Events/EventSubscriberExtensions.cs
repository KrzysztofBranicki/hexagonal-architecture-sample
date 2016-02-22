using System;
using System.Collections.Generic;
using System.Linq;

namespace Common.Domain.Events
{
    public static class EventSubscriberExtensions
    {
        public static IEnumerable<Type> GetHandledEventTypes<TEvent>(this IEventHandler<TEvent> eventHandler) where TEvent : class
        {
            var eventHandlerType = eventHandler.GetType();

            var handledEventNames = eventHandlerType
                .GetInterfaces()
                .Where(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEventHandler<>))
                .Select(x => x.GetGenericArguments().Single())
                .ToList();
            return handledEventNames;
        }
    }
}
