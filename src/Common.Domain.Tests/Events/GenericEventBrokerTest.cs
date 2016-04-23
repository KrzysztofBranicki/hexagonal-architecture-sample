using System;
using System.Linq;
using Common.Domain.Events;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace Common.Domain.Tests.Events
{
    [TestFixture]
    public abstract class GenericEventBrokerTest
    {
        protected abstract IEventBroker GetEventBroker(Func<Type, object> handlerInstanceCreator = null);

        private readonly Func<Type, object> _substituteCreatorFunction = type => Substitute.For(new[] { type }, new object[] { });

        [Test]
        public void Subscribed_event_handler_should_receive_published_event_of_same_type()
        {
            var eventBroker = GetEventBroker();
            var eventHandler = Substitute.For<IEventHandler<EventA>>();

            eventBroker.SubscribeHandlerInstance(eventHandler);
            var eventA = new EventA(Guid.NewGuid());
            eventBroker.Publish(eventA);

            eventHandler.Received(1).Handle(eventA);
        }

        [Test]
        public void Subscribed_event_handler_should_receive_published_event_of_same_type_with_type_registration()
        {
            var objectsCreator = ObjectsCreator.CreateNew(_substituteCreatorFunction);
            var eventBroker = new SimpleInProcEventBroker(objectsCreator.InstanceCreatorFunction);

            eventBroker.SubscribeHandlerType<IEventHandler<EventA>>();
            var eventA = new EventA(Guid.NewGuid());
            eventBroker.Publish(eventA);

            objectsCreator.CreatedInstances.Should().HaveCount(1);
            objectsCreator.CreatedInstances.Cast<IEventHandler<EventA>>().Single().Received(1).Handle(eventA);
        }

        [Test]
        public void Subscribed_event_handler_should_not_receive_events_which_are_not_supported_by_handler()
        {
            var eventBroker = GetEventBroker();
            var eventHandler = Substitute.For<IEventHandler<EventA>>();

            eventBroker.SubscribeHandlerInstance(eventHandler);
            var eventB = new EventB(Guid.NewGuid());
            eventBroker.Publish(eventB);

            eventHandler.DidNotReceiveWithAnyArgs().Handle(null);
        }

        [Test]
        public void Subscribed_event_handler_should_not_receive_events_which_are_not_supported_by_handler_with_type_registration()
        {
            var objectsCreator = ObjectsCreator.CreateNew(_substituteCreatorFunction);
            var eventBroker = new SimpleInProcEventBroker(objectsCreator.InstanceCreatorFunction);

            eventBroker.SubscribeHandlerType<IEventHandler<EventA>>();
            var eventB = new EventB(Guid.NewGuid());
            eventBroker.Publish(eventB);

            objectsCreator.CreatedInstances.Should().BeEmpty();
        }

        [Test]
        public void Subscribing_same_event_handler_instance_multiple_times_shouldnt_result_in_multiple_handle_calls_to_the_same_instance()
        {
            var eventBroker = GetEventBroker();
            var eventHandler = Substitute.For<IEventHandler<EventA>>();

            eventBroker.SubscribeHandlerInstance(eventHandler);
            eventBroker.SubscribeHandlerInstance(eventHandler);
            var eventA = new EventA(Guid.NewGuid());
            eventBroker.Publish(eventA);

            eventHandler.Received(1).Handle(eventA);
        }

        [Test]
        public void Subscribing_same_event_handler_instance_multiple_times_shouldnt_result_in_multiple_handle_calls_to_the_same_instance_with_type_registration()
        {
            var objectsCreator = ObjectsCreator.CreateNew(_substituteCreatorFunction);
            var eventBroker = new SimpleInProcEventBroker(objectsCreator.InstanceCreatorFunction);

            eventBroker.SubscribeHandlerType<IEventHandler<EventA>>();
            eventBroker.SubscribeHandlerType<IEventHandler<EventA>>();
            var eventA = new EventA(Guid.NewGuid());
            eventBroker.Publish(eventA);

            objectsCreator.CreatedInstances.Should().HaveCount(1);
            objectsCreator.CreatedInstances.Cast<IEventHandler<EventA>>().Single().Received(1).Handle(eventA);
        }

        [Test]
        public void After_unsubscribing_events_shouldnt_be_passed_to_handler()
        {
            var eventBroker = GetEventBroker();
            var eventHandler = Substitute.For<IEventHandler<EventA>>();
            eventBroker.SubscribeHandlerInstance(eventHandler);
            var eventA = new EventA(Guid.NewGuid());

            eventBroker.Publish(eventA);
            eventHandler.Received(1).Handle(eventA);

            eventBroker.Publish(eventA);
            eventHandler.Received(2).Handle(eventA);

            eventBroker.UnsubscribeHandlerInstance(eventHandler);
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

            eventBroker.SubscribeHandlerInstance(eventHandler1);
            eventBroker.SubscribeHandlerInstance(eventHandler2);
            eventBroker.SubscribeHandlerInstance(eventHandler3);

            var eventA = new EventA(Guid.NewGuid());
            var eventB = new EventB(Guid.NewGuid());

            eventBroker.Publish(eventA);
            eventBroker.Publish(eventB);

            eventHandler1.Received().Handle(eventA);
            eventHandler2.Received().Handle(eventA);
            eventHandler3.Received().Handle(eventB);
        }

        [Test]
        public void All_subscribed_handlers_should_receive_appropriate_events_with_type_registration()
        {
            var objectsCreator = ObjectsCreator.CreateNew(_substituteCreatorFunction);
            var eventBroker = new SimpleInProcEventBroker(objectsCreator.InstanceCreatorFunction);

            eventBroker.SubscribeHandlerType<IEventHandler<EventA>>();
            eventBroker.SubscribeHandlerType<IEventHandler<EventB>>();

            var eventA = new EventA(Guid.NewGuid());
            var eventA2 = new EventA(Guid.NewGuid());
            var eventB = new EventB(Guid.NewGuid());

            eventBroker.Publish(eventA);
            eventBroker.Publish(eventA2);
            eventBroker.Publish(eventB);

            objectsCreator.CreatedInstances.Should().HaveCount(3);
            ((IEventHandler<EventA>)objectsCreator.CreatedInstances.First()).Received(1).Handle(eventA);
            ((IEventHandler<EventA>)objectsCreator.CreatedInstances.Skip(1).First()).Received(1).Handle(eventA2);
            ((IEventHandler<EventB>)objectsCreator.CreatedInstances.Skip(2).First()).Received(1).Handle(eventB);
        }

        [Test]
        public void Handler_that_handles_multiple_event_types_should_receive_all_supported_events()
        {
            var eventBroker = GetEventBroker();
            var eventHandler = Substitute.For<IEventHandler<EventA>, IEventHandler<EventB>>();
            eventBroker.SubscribeHandlerInstance(eventHandler);

            var eventA = new EventA(Guid.NewGuid());
            var eventB = new EventB(Guid.NewGuid());

            eventBroker.Publish(eventA);
            eventBroker.Publish(eventB);

            eventHandler.ReceivedWithAnyArgs(2);
            eventHandler.Received(1).Handle(eventA);
            ((IEventHandler<EventB>)eventHandler).Received(1).Handle(eventB);
        }

        [Test]
        public void Handler_that_handles_multiple_event_types_should_receive_all_supported_events_with_type_registration()
        {
            var objectsCreator = ObjectsCreator.CreateNew();
            var eventBroker = new SimpleInProcEventBroker(objectsCreator.InstanceCreatorFunction);
            eventBroker.SubscribeHandlerType<MultiEventHandler>();

            var eventA = new EventA(Guid.NewGuid());
            var eventB = new EventB(Guid.NewGuid());

            eventBroker.Publish(eventA);
            eventBroker.Publish(eventB);

            objectsCreator.CreatedInstances.Should().HaveCount(2);
        }

        public class MultiEventHandler : IEventHandler<EventA>, IEventHandler<EventB>
        {
            public void Handle(EventA eventData)
            { }

            public void Handle(EventB eventData)
            { }
        }
    }
}
