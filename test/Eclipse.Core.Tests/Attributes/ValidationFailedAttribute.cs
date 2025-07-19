using Eclipse.Core.Validation;

namespace Eclipse.Core.Tests.Attributes;

internal class ValidationFailedAttribute : ContextValidationAttribute
{
    public override ValidationResult Validate(ValidationContext context)
    {
        return ValidationResult.Failure("Not allowed");
    }
}
