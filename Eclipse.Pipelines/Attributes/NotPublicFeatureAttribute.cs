using Eclipse.Core.Attributes;
using Eclipse.Core.Validation;

namespace Eclipse.Pipelines.Attributes;

internal class NotPublicFeatureAttribute : ContextValidationAttribute
{
    public override ValidationResult Validate(ValidationContext context)
    {
        return ValidationResult.Failure("Not allowed");
    }
}
