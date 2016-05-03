using System;
using Common.Domain.Events;
using NSubstitute;
using NUnit.Framework;

namespace Common.Domain.Tests.Events
{
    [TestFixture]
    public class SimpleInProcEventBrokerTest : GenericEventBrokerTest
    {
        protected override IEventBroker GetEventBroker(Func<Type, object> handlerInstanceCreator = null)
        {
            return new SimpleInProcEventBroker(handlerInstanceCreator);
        }

        [Test]
        public void Handler_that_handles_base_event_should_receive_calls_with_derived_event_also() //This is currently implementation specific behavior
        {
            var eventBroker = GetEventBroker();
            var eventHandlerForBaseEvent = Substitute.For<IEventHandler<BaseTestEvent>>();
            eventBroker.SubscribeHandlerInstance(eventHandlerForBaseEvent);

            var baseEvent = new BaseTestEvent(Guid.NewGuid());
            var eventA = new EventA(Guid.NewGuid());
            var eventB = new EventB(Guid.NewGuid());

            eventBroker.Publish(baseEvent);
            eventBroker.Publish(eventA);
            eventBroker.Publish(eventB);

            eventHandlerForBaseEvent.ReceivedWithAnyArgs(3).Handle(null);
            eventHandlerForBaseEvent.Received(1).Handle(baseEvent);
            eventHandlerForBaseEvent.Received(1).Handle(eventA);
            eventHandlerForBaseEvent.Received(1).Handle(eventB);
        }
    }
}
