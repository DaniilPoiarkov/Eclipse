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
        if (context.TelegramUser is null)
        {
            return ValidationResult.Failure("User is null");
        }

        var options = context.ServiceProvider.GetRequiredService<IOptions<TelegramOptions>>().Value;

        if (options is null)
        {
            return ValidationResult.Failure("Options not provided");
        }

        if (options.Chat != context.TelegramUser.Id)
        {
            return ValidationResult.Failure("Access not allowed");
        }

        return ValidationResult.Success();
    }
}
