using Eclipse.Core.Validation;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Eclipse.Pipelines.Attributes;

internal sealed class AdminOnlyAttribute : ContextValidationAttribute
{
    public override ValidationResult Validate(ValidationContext context)
    {
        // Just ignore. We don't let users know that this is existing command

        if (context.Update.Type != UpdateType.Message || context.Update.Message is not Message message)
        {
            return ValidationResult.Failure("Pipelines:NotFound");
        }

        var options = context.ServiceProvider.GetRequiredService<IOptions<PipelinesOptions>>().Value;

        if (options.Chat != message.From?.Id)
        {
            return ValidationResult.Failure("Pipelines:NotFound");
        }

        return ValidationResult.Success();
    }
}
