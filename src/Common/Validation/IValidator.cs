namespace Common.Validation
{
    public interface IValidator
    {
        ValidationResult Validate(object objectToValidate);
        void AssertIsValid(object objectToValidate);
    }
}
