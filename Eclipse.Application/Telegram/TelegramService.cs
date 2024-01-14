using Eclipse.Application.Contracts.Telegram;
using Eclipse.Application.Exceptions;

using FluentValidation;

using Telegram.Bot;

namespace Eclipse.Application.Telegram;

internal class TelegramService : ITelegramService
{
    private readonly ITelegramBotClient _botClient;

    public TelegramService(ITelegramBotClient botClient)
    {
        _botClient = botClient;
    }

    public async Task Send(SendMessageModel message, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(message.Message))
        {
            throw new EclipseValidationException(
                [TelegramErrors.Messages.MessageCannotBeEmpty]
            );
        }

        if (message.ChatId == default)
        {
            throw new EclipseValidationException([]);
        }

        await _botClient.SendTextMessageAsync(
            message.ChatId,
            message.Message,
            cancellationToken: cancellationToken);
    }
}
