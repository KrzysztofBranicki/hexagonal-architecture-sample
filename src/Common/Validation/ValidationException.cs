using System;

namespace Common.Validation
{
    [Serializable]
    public class ValidationException : Exception
    {
        public ValidationResult ValidationResult { get; private set; }

        public ValidationException(ValidationResult validationResult) : base(validationResult.ErrorMessage)
        {
            ValidationResult = validationResult;
        }
    }
}
