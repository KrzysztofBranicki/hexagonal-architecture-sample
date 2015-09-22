using System;

namespace Common.Validation
{
    public class ValidationError
    {
        public string ErrorMessage { get; }
        public string MemberName { get; }

        public ValidationError(string errorMessage, string memberName = null)
        {
            if (errorMessage == null) throw new ArgumentNullException(nameof(errorMessage));

            ErrorMessage = errorMessage;
            MemberName = memberName;
        }
    }
}