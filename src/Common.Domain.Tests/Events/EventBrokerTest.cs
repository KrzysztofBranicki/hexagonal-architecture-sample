using Common.Domain.Events;
using NSubstitute;
using NUnit.Framework;

namespace Common.Domain.Tests.Events
{
    [TestFixture]
    public class EventBrokerTest
    {
        [Test]
        public void All_subscribed_handlers_should_receive_appropriate_events()
        {
            var eventBroker = new EventBroker();

            var eventHandler1 = Substitute.For<IEventHandler<TestEvent1>>();
            var eventHandler2 = Substitute.For<IEventHandler<TestEvent1>>();
            var eventHandler3 = Substitute.For<IEventHandler<TestEvent2>>();

            eventBroker.SubscribeEventHandler(eventHandler1);
            eventBroker.SubscribeEventHandler(eventHandler2);
            eventBroker.SubscribeEventHandler(eventHandler3);

            var testEvent1 = new TestEvent1();
            var testEvent2 = new TestEvent2();

            eventBroker.Publish(testEvent1);
            eventBroker.Publish(testEvent2);

            eventHandler1.Received().Handle(testEvent1);
            eventHandler2.Received().Handle(testEvent1);
            eventHandler3.Received().Handle(testEvent2);
        }

        [Test]
        public void After_unsubscribing_events_shouldnt_be_passed_to_handler()
        {
            var eventBroker = new EventBroker();
            var eventHandler = Substitute.For<IEventHandler<TestEvent1>>();
            eventBroker.SubscribeEventHandler(eventHandler);
            var testEvent = new TestEvent1();

            eventBroker.Publish(testEvent);
            eventHandler.Received(1).Handle(testEvent);

            eventBroker.Publish(testEvent);
            eventHandler.Received(2).Handle(testEvent);

            eventBroker.UnsubscribeEventHandler(eventHandler);
            eventBroker.Publish(testEvent);
            eventHandler.Received(2).Handle(testEvent);
        }
    }

    public class TestEvent1
    { }

    public class TestEvent2
    { }
}
