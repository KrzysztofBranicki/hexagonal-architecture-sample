using System.Collections.Generic;
using System.Threading;

namespace Common.Domain.Events
{
    public class EventBroker : IEventPublisher, IEventSubscriber
    {
        private HashSet<IEventHandler> _eventHandlers = new HashSet<IEventHandler>();
        private readonly object _syncObject = new object();

        public void Publish<T>(T eventObject) where T : class
        {
            foreach (var handler in _eventHandlers)
            {
                var correctHandler = handler as IEventHandler<T>;
                correctHandler?.Handle(eventObject);
            }
        }

        public void SubscribeEventHandler(IEventHandler eventHandler)
        {
            lock (_syncObject)
            {
                var newEventHandlers = new HashSet<IEventHandler>(_eventHandlers) { eventHandler };
                Volatile.Write(ref _eventHandlers, newEventHandlers);
            }
        }

        public void UnsubscribeEventHandler(IEventHandler eventHandler)
        {
            lock (_syncObject)
            {
                var newEventHandlers = new HashSet<IEventHandler>(_eventHandlers);
                newEventHandlers.Remove(eventHandler);
                Volatile.Write(ref _eventHandlers, newEventHandlers);
            }
        }

        public static readonly EventBroker DefaultInstance = new EventBroker();
    }
}
