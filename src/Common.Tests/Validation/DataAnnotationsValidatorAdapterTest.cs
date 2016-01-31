using System.Linq;
using Common.Validation;
using NUnit.Framework;

namespace Common.Tests.Validation
{
    [TestFixture]
    public class DataAnnotationsValidatorAdapterTest
    {
        private readonly IValidator _validator = new DataAnnotationsValidatorAdapter();

        private readonly ClassToValidate _validObject = new ClassToValidate { FirstRequiredField = "Value", SecondRequiredField = "value"};
        private readonly ClassToValidate _invalidObject = new ClassToValidate();

        [Test]
        public void Assert_is_valid_should_throw_validation_exception_when_object_is_invalid()
        {
            Assert.That(() => _validator.AssertIsValid(_invalidObject), Throws.InstanceOf<ValidationException>());
        }

        [Test]
        public void Assert_is_valid_should_not_throw_exception_when_object_is_valid()
        {
            _validator.AssertIsValid(_validObject);
        }

        [Test]
        public void Validation_should_result_in_two_missing_required_fields()
        {
            var vr = _validator.Validate(_invalidObject);
            Assert.That(vr.IsValid, Is.False);
            Assert.That(vr.ValidationErrors.Count(), Is.EqualTo(2));
        }

        [Test]
        public void Validation_should_result_in_one_missing_required_fields()
        {
            var objectWithOneMissingRequiredField = new ClassToValidate
            {
                FirstRequiredField = "value"
            };

            var vr = _validator.Validate(objectWithOneMissingRequiredField);
            Assert.That(vr.IsValid, Is.False);
            Assert.That(vr.ValidationErrors.Count(), Is.EqualTo(1));
            Assert.That(vr.ValidationErrors.First().MemberName, Is.EqualTo("SecondRequiredField"));
            Assert.That(vr.ValidationErrors.First().ErrorMessage, Is.EqualTo(ClassToValidate.RequiredFieldErrorMessage));
        }

        [Test]
        public void Validation_result_of_valid_object_should_be_valid()
        {
            var vr = _validator.Validate(_validObject);
            Assert.That(vr.IsValid, Is.True);
            Assert.That(vr.ValidationErrors, Is.Empty);
            Assert.That(vr.ErrorMessage, Is.Null);
        }
    }
}
