using System;
using System.Threading;
using Common.Domain.Events;

namespace Common.Domain.Tests.Events
{
    public class EventBrokerDelayAfterPublishDecorator : IEventBroker
    {
        private readonly IEventBroker _decoratedEventBrokek;
        private readonly TimeSpan _delayTimeSpan;

        public EventBrokerDelayAfterPublishDecorator(IEventBroker decoratedEventBrokek, TimeSpan delayTimeSpan)
        {
            _decoratedEventBrokek = decoratedEventBrokek;
            _delayTimeSpan = delayTimeSpan;
        }

        public void Publish<T>(T eventObject) where T : class
        {
            _decoratedEventBrokek.Publish(eventObject);

            //TODO refactor - this is currently creating delay after event was published to give event handler a chance to handle event before assertion in test is run
            Thread.Sleep(_delayTimeSpan);
        }

        public void SubscribeEventHandler<TEvent>(IEventHandler<TEvent> eventHandler) where TEvent : class
        {
            _decoratedEventBrokek.SubscribeEventHandler(eventHandler);
        }

        public void UnsubscribeEventHandler<TEvent>(IEventHandler<TEvent> eventHandler) where TEvent : class
        {
            _decoratedEventBrokek.UnsubscribeEventHandler(eventHandler);
        }
    }
}