namespace Common.Domain.Events
{
    public interface IEventBroker : IEventPublisher, IEventSubscriber
    { }
}
