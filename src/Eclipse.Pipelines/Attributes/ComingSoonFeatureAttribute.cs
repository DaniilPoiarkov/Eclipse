using Eclipse.Core.Validation;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Eclipse.Pipelines.Attributes;

internal sealed class ComingSoonFeatureAttribute : ContextValidationAttribute
{
    public override ValidationResult Validate(ValidationContext context)
    {
        var options = context.ServiceProvider.GetRequiredService<IOptions<PipelinesOptions>>();

        if (context.TelegramUser is not null && context.TelegramUser.Id == options.Value.Chat)
        {
            return ValidationResult.Success();
        }

        return ValidationResult.Failure("ComingSoonFeature");
    }
}
