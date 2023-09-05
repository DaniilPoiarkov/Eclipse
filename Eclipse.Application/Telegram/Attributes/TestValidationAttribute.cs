using Eclipse.Core.Attributes;
using Eclipse.Core.Validation;

namespace Eclipse.Application.Telegram.Attributes;

internal class TestValidationAttribute : ContextValidationAttribute
{
    public override ValidationResult Validate(ValidationContext context)
    {
        return ValidationResult.Failure("Test");
    }
}
