using Eclipse.Core.Attributes;
using Eclipse.Core.Validation;

namespace Eclipse.Pipelines.Attributes;

internal sealed class FeatureFlagAttribute : ContextValidationAttribute
{
    private readonly bool _isEnabled;

    public FeatureFlagAttribute(bool isEnabled)
    {
        _isEnabled = isEnabled;
    }

    public override ValidationResult Validate(ValidationContext context)
    {
        return _isEnabled
            ? ValidationResult.Success()
            : ValidationResult.Failure();
    }
}
