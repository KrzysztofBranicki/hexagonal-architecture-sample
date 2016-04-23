using System;
using Common.Domain.Events;
using FluentAssertions;
using NUnit.Framework;

namespace Common.Domain.Tests.Events
{
    [TestFixture]
    public class EventHandlerExtensionsTests
    {
        [Test]
        public void Should_get_all_handled_events_form_class()
        {
            var supportedEvents = EventHandlerExtensions.GetEventTypesWhichHandlerSupports<TestHandler>();

            supportedEvents.Should().HaveCount(2);
            supportedEvents.Should().Contain(typeof(EventA));
            supportedEvents.Should().Contain(typeof(EventB));
        }

        [Test]
        public void Should_get_all_handled_events_form_interface()
        {
            var supportedEvents = EventHandlerExtensions.GetEventTypesWhichHandlerSupports<IEventHandler<EventB>>();

            supportedEvents.Should().HaveCount(1);
            supportedEvents.Should().Contain(typeof(EventB));
        }


        class TestHandler : IEventHandler<EventA>, IEventHandler<EventB>
        {
            public void Handle(EventA eventData)
            {
                throw new NotImplementedException();
            }

            public void Handle(EventB eventData)
            {
                throw new NotImplementedException();
            }
        }
    }
}
