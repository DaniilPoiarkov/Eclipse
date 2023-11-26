using Eclipse.Application.Contracts.Telegram;

using FluentValidation;

namespace Eclipse.Application.Telegram;

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
