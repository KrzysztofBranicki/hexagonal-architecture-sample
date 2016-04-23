using System;

namespace Common.Domain.Tests.Events
{
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
