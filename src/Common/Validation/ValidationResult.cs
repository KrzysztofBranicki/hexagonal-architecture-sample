using System.Collections.Generic;
using System.Linq;

namespace Common.Validation
{
    public class ValidationResult
    {
        public IEnumerable<ValidationError> ValidationErrors { get; }
        public bool IsValid => !ValidationErrors.Any();
        public string ErrorMessage => IsValid ? null : ValidationErrors.First().ErrorMessage;

        public ValidationResult Merge(ValidationResult other)
        {
            return new ValidationResult(ValidationErrors.Union(other.ValidationErrors));
        }

        protected ValidationResult()
        {
            ValidationErrors = Enumerable.Empty<ValidationError>();
        }

        public ValidationResult(IEnumerable<ValidationError> validationErrors)
        {
            ValidationErrors = validationErrors;
        }

        public static ValidationResult CreateInvalidResult(IEnumerable<ValidationError> validationErrors)
        {
            return new ValidationResult(validationErrors);
        }

        public static ValidationResult CreateInvalidResult(params ValidationError[] validationErrors)
        {
            return new ValidationResult(validationErrors);
        }

        public static readonly ValidationResult ValidResult = new ValidationResult();
    }
}
