using Eclipse.Infrastructure.Telegram;
using Telegram.Bot;

namespace Eclipse.Infrastructure.Internals.Telegram;

internal class TelegramService : ITelegramService
{
    private readonly ITelegramBotClient _botClient;

    public TelegramService(ITelegramBotClient botClient)
    {
        _botClient = botClient;
    }

    public async Task Send(SendMessageModel message, CancellationToken cancellationToken = default)
    {
        await _botClient.SendTextMessageAsync(
            message.ChatId,
            message.Message,
            cancellationToken: cancellationToken);
    }
}
