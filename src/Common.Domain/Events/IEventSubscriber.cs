namespace Common.Domain.Events
{
    interface IEventSubscriber
    {
        void SubscribeEventHandler(IEventHandler eventHandler);
        void UnsubscribeEventHandler(IEventHandler eventHandler);
    }
}
