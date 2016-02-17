namespace Common.Domain.Events
{
    public interface IEventHandler<in TEvent> where TEvent : class
    {
        void Handle(TEvent eventData);
    }
}
