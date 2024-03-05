using Eclipse.Common.Results;
using Eclipse.Telegram.Application.Contracts;

using Telegram.Bot;

namespace Eclipse.Telegram.Application;

internal sealed class TelegramService : ITelegramService
{
    private readonly ITelegramBotClient _botClient;

    private static readonly string _errorCode = "Telegram.Send";

    public TelegramService(ITelegramBotClient botClient)
    {
        _botClient = botClient;
    }

    public async Task<Result> Send(SendMessageModel message, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(message.Message))
        {
            return Error.Validation(_errorCode, "Telegram:MessageCannotBeEmpty");
        }

        if (message.ChatId == default)
        {
            return Error.Validation(_errorCode, "Telegram:InvalidChatId");
        }

        await _botClient.SendTextMessageAsync(
            message.ChatId,
            message.Message,
            cancellationToken: cancellationToken);

        return Result.Success();
    }
}