using System;

namespace Common.Domain
{
    [Serializable]
    public class DomainInvariantViolatedException : Exception
    {
        public string RelatedMember { get; }

        public DomainInvariantViolatedException(string message, string relatedMember = null)
            : base(message)
        {
            RelatedMember = relatedMember;
        }
    }
}
