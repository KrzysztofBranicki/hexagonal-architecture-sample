using System.ComponentModel.DataAnnotations;

namespace Common.Tests.Validation
{
    public class ClassToValidate
    {
        public const string RequiredFieldErrorMessage = "RequiredFieldErrorMessage";

        [Required(ErrorMessage = RequiredFieldErrorMessage)]
        public string FirstRequiredField { get; set; }
        [Required(ErrorMessage = RequiredFieldErrorMessage)]
        public string SecondRequiredField { get; set; }

        public string FirstNotRequiredField { get; set; }
        public string SecondNotRequiredField { get; set; }
    }
}
