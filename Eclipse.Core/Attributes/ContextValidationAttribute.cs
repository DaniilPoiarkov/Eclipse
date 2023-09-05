using Eclipse.Core.Validation;

namespace Eclipse.Core.Attributes;

public abstract class ContextValidationAttribute : Attribute
{
    public abstract ValidationResult Validate(ValidationContext context);
}
