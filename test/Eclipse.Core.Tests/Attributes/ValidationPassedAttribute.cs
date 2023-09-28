using Eclipse.Core.Attributes;
using Eclipse.Core.Validation;

namespace Eclipse.Core.Tests.Attributes;

internal class ValidationPassedAttribute : ContextValidationAttribute
{
    public override ValidationResult Validate(ValidationContext context)
    {
        return ValidationResult.Success();
    }
}
