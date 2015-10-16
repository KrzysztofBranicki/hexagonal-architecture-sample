namespace Common.Domain.Events
{
    public interface IEventPublisher
    {
        void Publish<T>(T eventObject) where T : class;
    }
}
