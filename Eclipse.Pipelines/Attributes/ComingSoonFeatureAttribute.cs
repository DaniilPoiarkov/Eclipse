using Eclipse.Core.Attributes;
using Eclipse.Core.Validation;

namespace Eclipse.Pipelines.Attributes;

internal class ComingSoonFeatureAttribute : ContextValidationAttribute
{
    public override ValidationResult Validate(ValidationContext context)
    {
        return ValidationResult.Failure("This feature is coming soon. Stay ahead!");
    }
}
