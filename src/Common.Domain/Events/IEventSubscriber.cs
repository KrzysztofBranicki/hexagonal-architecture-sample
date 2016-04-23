namespace Common.Domain.Events
{
    public interface IEventSubscriber
    {
        void SubscribeHandlerInstance<TEvent>(IEventHandler<TEvent> eventHandler) where TEvent : class;
        void UnsubscribeHandlerInstance<TEvent>(IEventHandler<TEvent> eventHandler) where TEvent : class;
        void SubscribeHandlerType<TEventHandler>() where TEventHandler : IEventHandler;
    }
}
