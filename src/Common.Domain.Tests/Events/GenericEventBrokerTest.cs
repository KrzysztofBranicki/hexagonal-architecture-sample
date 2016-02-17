using System;
using Common.Domain.Events;
using NSubstitute;
using NUnit.Framework;

namespace Common.Domain.Tests.Events
{
    [TestFixture]
    public abstract class GenericEventBrokerTest
    {
        protected abstract IEventBroker GetEventBroker();

        [Test]
        public void Subscribed_event_handler_should_receive_published_event_of_same_type()
        {
            var eventBroker = GetEventBroker();
            var eventHandler = Substitute.For<IEventHandler<EventA>>();

            eventBroker.SubscribeEventHandler(eventHandler);
            var eventA = new EventA(Guid.NewGuid());
            eventBroker.Publish(eventA);

            eventHandler.Received(1).Handle(eventA);
        }

        [Test]
        public void Subscribed_event_handler_should_not_receive_events_which_are_not_supported_by_handler()
        {
            var eventBroker = GetEventBroker();
            var eventHandler = Substitute.For<IEventHandler<EventA>>();

            eventBroker.SubscribeEventHandler(eventHandler);
            var eventB = new EventB(Guid.NewGuid());
            eventBroker.Publish(eventB);

            eventHandler.DidNotReceiveWithAnyArgs().Handle(null);
        }

        [Test]
        public void Subscribing_same_event_handler_instance_multiple_times_shouldnt_result_in_multiple_handle_calls_to_the_same_instance()
        {
            var eventBroker = GetEventBroker();
            var eventHandler = Substitute.For<IEventHandler<EventA>>();

            eventBroker.SubscribeEventHandler(eventHandler);
            eventBroker.SubscribeEventHandler(eventHandler);
            var eventA = new EventA(Guid.NewGuid());
            eventBroker.Publish(eventA);

            eventHandler.Received(1).Handle(eventA);
        }

        [Test]
        public void After_unsubscribing_events_shouldnt_be_passed_to_handler()
        {
            var eventBroker = GetEventBroker();
            var eventHandler = Substitute.For<IEventHandler<EventA>>();
            eventBroker.SubscribeEventHandler(eventHandler);
            var eventA = new EventA(Guid.NewGuid());

            eventBroker.Publish(eventA);
            eventHandler.Received(1).Handle(eventA);

            eventBroker.Publish(eventA);
            eventHandler.Received(2).Handle(eventA);

            eventBroker.UnsubscribeEventHandler(eventHandler);
            eventBroker.Publish(eventA);
            eventHandler.Received(2).Handle(eventA);
        }

        [Test]
        public void All_subscribed_handlers_should_receive_appropriate_events()
        {
            var eventBroker = GetEventBroker();

            var eventHandler1 = Substitute.For<IEventHandler<EventA>>();
            var eventHandler2 = Substitute.For<IEventHandler<EventA>>();
            var eventHandler3 = Substitute.For<IEventHandler<EventB>>();

            eventBroker.SubscribeEventHandler(eventHandler1);
            eventBroker.SubscribeEventHandler(eventHandler2);
            eventBroker.SubscribeEventHandler(eventHandler3);

            var eventA = new EventA(Guid.NewGuid());
            var eventB = new EventB(Guid.NewGuid());

            eventBroker.Publish(eventA);
            eventBroker.Publish(eventB);

            eventHandler1.Received().Handle(eventA);
            eventHandler2.Received().Handle(eventA);
            eventHandler3.Received().Handle(eventB);
        }

        [Test]
        public void Handler_that_handles_multiple_event_types_should_receive_all_supported_events()
        {
            var eventBroker = GetEventBroker();
            var eventHandler = Substitute.For<IEventHandler<EventA>, IEventHandler<EventB>>();
            eventBroker.SubscribeEventHandler(eventHandler);

            var eventA = new EventA(Guid.NewGuid());
            var eventB = new EventB(Guid.NewGuid());

            eventBroker.Publish(eventA);
            eventBroker.Publish(eventB);

            eventHandler.ReceivedWithAnyArgs(2);
            eventHandler.Received(1).Handle(eventA);
            ((IEventHandler<EventB>)eventHandler).Received(1).Handle(eventB);
        }

        public class BaseTestEvent : IEquatable<BaseTestEvent>
        {
            public Guid Id { get; }

            public BaseTestEvent(Guid id)
            {
                Id = id;
            }

            public bool Equals(BaseTestEvent other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return Id.Equals(other.Id);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((BaseTestEvent)obj);
            }

            public override int GetHashCode()
            {
                return Id.GetHashCode();
            }
        }

        public class EventA : BaseTestEvent
        {
            public EventA(Guid id) : base(id)
            {
            }
        }

        public class EventB : BaseTestEvent
        {
            public EventB(Guid id) : base(id)
            {
            }
        }
    }
}
