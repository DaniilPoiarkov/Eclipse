using Microsoft.Extensions.Options;
using Telegram.Bot;

namespace Eclipse.WebAPI.Services.TelegramServices.Implementations;

public class TelegramService : ITelegramService
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
