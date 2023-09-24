using Eclipse.Application.Contracts.Telegram;
using Eclipse.Localization.Exceptions;

using FluentValidation;

using Telegram.Bot;

namespace Eclipse.Application.Telegram;

internal class TelegramService : ITelegramService
{
    private readonly ITelegramBotClient _botClient;

    private readonly IValidator<SendMessageModel> _validator;

    public TelegramService(ITelegramBotClient botClient, IValidator<SendMessageModel> validator)
    {
        _botClient = botClient;
        _validator = validator;
    }

    public async Task Send(SendMessageModel message, CancellationToken cancellationToken = default)
    {
        var validationResult = _validator.Validate(message);

        if (!validationResult.IsValid)
        {
            throw new LocalizedException("Eclipse:ValidationFailed", validationResult.Errors.Select(e => e.ErrorMessage).ToArray());
        }

        await _botClient.SendTextMessageAsync(
            message.ChatId,
            message.Message,
            cancellationToken: cancellationToken);
    }
}
