using Eclipse.Infrastructure.Telegram;

using FluentValidation;

namespace Eclipse.Infrastructure.Internals.Telegram;

public class SendMessageModelValidator : AbstractValidator<SendMessageModel>
{
    public SendMessageModelValidator()
    {
        RuleFor(m => m.Message)
            .NotEmpty()
            .WithMessage(TelegramErrors.Messages.MessageCannotBeEmpty);

        RuleFor(m => m.ChatId)
            .NotEqual(0);
    }
}
