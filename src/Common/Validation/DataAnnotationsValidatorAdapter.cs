using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

using DataAnnotationsValidationResult = System.ComponentModel.DataAnnotations.ValidationResult;

namespace Common.Validation
{
    public class DataAnnotationsValidatorAdapter : IValidator
    {
        public ValidationResult Validate(object objectToValidate)
        {
            var result = new List<DataAnnotationsValidationResult>();
            var ctx = new ValidationContext(objectToValidate, null, null);
            var isValid = Validator.TryValidateObject(objectToValidate, ctx, result, true);
            return isValid ? ValidationResult.ValidResult : CreateValidationResult(result);
        }

        public void AssertIsValid(object objectToValidate)
        {
            if (objectToValidate == null)
                return;

            var vr = Validate(objectToValidate);
            if (!vr.IsValid)
                throw new ValidationException(vr);
        }

        private static ValidationResult CreateValidationResult(IEnumerable<DataAnnotationsValidationResult> validationResults)
        {
            var errors = validationResults.Where(x => x != DataAnnotationsValidationResult.Success)
                .SelectMany(x => x.MemberNames.Select(m => new ValidationError(x.ErrorMessage, m)));

            return ValidationResult.CreateInvalidResult(errors);
        }
    }
}
