using Eclipse.Application.Contracts.Telegram.BotManagement;
using FluentValidation;

namespace Eclipse.Application.Telegram.Commands;

public class CommandDtoValidator : AbstractValidator<CommandDto>
{
    public CommandDtoValidator()
    {
        RuleFor(c => c.Command)
            .NotNull()
            .NotEmpty()
            .MinimumLength(1)
            .MaximumLength(24);

        RuleFor(c => c.Description)
            .NotEmpty()
            .NotNull()
            .MinimumLength(10)
            .MaximumLength(256);
    }
}
