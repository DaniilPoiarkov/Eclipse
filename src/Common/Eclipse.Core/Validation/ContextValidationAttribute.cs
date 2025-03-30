namespace Eclipse.Core.Validation;

public abstract class ContextValidationAttribute : Attribute
{
    public abstract ValidationResult Validate(ValidationContext context);
}
