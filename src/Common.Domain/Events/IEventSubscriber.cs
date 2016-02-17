namespace Common.Domain.Events
{
    public interface IEventSubscriber
    {
        void SubscribeEventHandler<TEvent>(IEventHandler<TEvent> eventHandler) where TEvent : class;
        void UnsubscribeEventHandler<TEvent>(IEventHandler<TEvent> eventHandler) where TEvent : class;
    }
}
