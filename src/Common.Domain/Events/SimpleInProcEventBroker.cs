using System.Collections.Generic;
using System.Threading;

namespace Common.Domain.Events
{
    public class SimpleInProcEventBroker : IEventBroker
    {
        private HashSet<object> _eventHandlers = new HashSet<object>();
        private readonly object _syncObject = new object();

        public void Publish<T>(T eventObject) where T : class
        {
            foreach (var handler in _eventHandlers)
            {
                var correctHandler = handler as IEventHandler<T>;
                correctHandler?.Handle(eventObject);
            }
        }

        public void SubscribeEventHandler<TEvent>(IEventHandler<TEvent> eventHandler) where TEvent : class
        {
            lock (_syncObject)
            {
                var newEventHandlers = new HashSet<object>(_eventHandlers) { eventHandler };
                Volatile.Write(ref _eventHandlers, newEventHandlers);
            }
        }

        public void UnsubscribeEventHandler<TEvent>(IEventHandler<TEvent> eventHandler) where TEvent : class
        {
            lock (_syncObject)
            {
                var newEventHandlers = new HashSet<object>(_eventHandlers);
                newEventHandlers.Remove(eventHandler);
                Volatile.Write(ref _eventHandlers, newEventHandlers);
            }
        }

        public static readonly SimpleInProcEventBroker DefaultInstance = new SimpleInProcEventBroker();
    }
}
