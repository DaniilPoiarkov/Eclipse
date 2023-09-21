using Eclipse.Application.Contracts;
using Eclipse.Application.Contracts.Telegram.Commands;
using FluentValidation;

namespace Eclipse.Application.Telegram.Commands;

public class CommandDtoValidator : AbstractValidator<CommandDto>
{
    public CommandDtoValidator()
    {
        RuleFor(c => c.Command)
            .NotNull()
            .NotEmpty()
            .MinimumLength(EclipseConstants.BotCommandConstants.CommandMinLength)
                .WithMessage(EclipseApplicationErrors.BotCommands.Messages.CommandMinLength)
            .MaximumLength(EclipseConstants.BotCommandConstants.CommandMaxLength)
                .WithMessage(EclipseApplicationErrors.BotCommands.Messages.CommandMaxLength);

        RuleFor(c => c.Description)
            .NotEmpty()
            .NotNull()
            .MinimumLength(EclipseConstants.BotCommandConstants.DescriptionMinLength)
                .WithMessage(EclipseApplicationErrors.BotCommands.Messages.DescriptionMinLength)
            .MaximumLength(EclipseConstants.BotCommandConstants.DescriptionMaxLength)
                .WithMessage(EclipseApplicationErrors.BotCommands.Messages.DescriptionMaxLength);
    }
}
