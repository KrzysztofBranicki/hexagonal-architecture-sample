using System;
using System.Collections.Generic;
using System.Threading;

namespace Common.Domain.Events
{
    public class SimpleInProcEventBroker : IEventBroker
    {
        private HashSet<object> _eventHandlerInstances = new HashSet<object>();
        private readonly object _syncObject = new object();

        private readonly Dictionary<string, HashSet<Type>> _handlersTypesForEventType = new Dictionary<string, HashSet<Type>>();
        private readonly Func<Type, object> _handlerInstanceCreator;

        public SimpleInProcEventBroker(Func<Type, object> handlerInstanceCreator = null)
        {
            _handlerInstanceCreator = handlerInstanceCreator ?? Activator.CreateInstance;
        }

        public void Publish<T>(T eventObject) where T : class
        {
            //Publish to registered event handler instances
            foreach (var handler in _eventHandlerInstances)
            {
                var correctHandler = handler as IEventHandler<T>;
                correctHandler?.Handle(eventObject);
            }

            //Publish to registered event handler types
            var eventName = eventObject.GetType().FullName;
            if (_handlersTypesForEventType.ContainsKey(eventName))
            {
                foreach (var handlerType in _handlersTypesForEventType[eventName])
                {
                    var handler = (IEventHandler<T>)_handlerInstanceCreator(handlerType);
                    handler.Handle(eventObject);
                }
            }
        }

        public void SubscribeHandlerInstance<TEvent>(IEventHandler<TEvent> eventHandler) where TEvent : class
        {
            lock (_syncObject)
            {
                var newEventHandlers = new HashSet<object>(_eventHandlerInstances) { eventHandler };
                Volatile.Write(ref _eventHandlerInstances, newEventHandlers);
            }
        }

        public void UnsubscribeHandlerInstance<TEvent>(IEventHandler<TEvent> eventHandler) where TEvent : class
        {
            lock (_syncObject)
            {
                var newEventHandlers = new HashSet<object>(_eventHandlerInstances);
                newEventHandlers.Remove(eventHandler);
                Volatile.Write(ref _eventHandlerInstances, newEventHandlers);
            }
        }

        public void SubscribeHandlerType<TEventHandler>() where TEventHandler : IEventHandler
        {
            var handlerType = typeof(TEventHandler);
            var handledEvents = EventHandlerExtensions.GetEventTypesWhichHandlerSupports(handlerType);
            foreach (var handledEvent in handledEvents)
            {
                var enetName = handledEvent.FullName;
                if (_handlersTypesForEventType.ContainsKey(enetName))
                {
                    _handlersTypesForEventType[enetName].Add(handlerType);
                }
                else
                {
                    _handlersTypesForEventType[enetName] = new HashSet<Type> { handlerType };
                }
            }
        }
    }
}
