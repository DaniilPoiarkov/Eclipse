using Eclipse.Common.Telegram;
using Eclipse.Core.Attributes;
using Eclipse.Core.Validation;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Eclipse.Pipelines.Attributes;

internal sealed class AdminOnlyAttribute : ContextValidationAttribute
{
    public override ValidationResult Validate(ValidationContext context)
    {
        // Just ignore. We don't let users know that this is existing command

        if (context.TelegramUser is null)
        {
            return ValidationResult.Failure("Pipelines:NotFound");
        }

        var options = context.ServiceProvider.GetRequiredService<IOptions<TelegramOptions>>().Value;

        if (options is null)
        {
            return ValidationResult.Failure("Pipelines:NotFound");
        }

        if (options.Chat != context.TelegramUser.Id)
        {
            return ValidationResult.Failure("Pipelines:NotFound");
        }

        return ValidationResult.Success();
    }
}
