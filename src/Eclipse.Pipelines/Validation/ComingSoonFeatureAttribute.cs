﻿using Eclipse.Core.Validation;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Eclipse.Pipelines.Validation;

internal sealed class ComingSoonFeatureAttribute : ContextValidationAttribute
{
    public override ValidationResult Validate(ValidationContext context)
    {
        if (context.Update.Type != UpdateType.Message || context.Update.Message is not Message message)
        {
            return ValidationResult.Failure("Pipelines:NotFound");
        }

        var options = context.ServiceProvider.GetRequiredService<IOptions<PipelinesOptions>>();

        if (message.From?.Id == options.Value.Chat)
        {
            return ValidationResult.Success();
        }

        return ValidationResult.Failure("ComingSoonFeature");
    }
}
