namespace Common.Domain.Events
{
    public interface IEventHandler
    { }

    public interface IEventHandler<in TEvent> : IEventHandler where TEvent : class
    {
        void Handle(TEvent eventData);
    }
}
