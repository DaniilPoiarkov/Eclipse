using Microsoft.Extensions.Options;
using Telegram.Bot;

namespace Eclipse.WebAPI.Services.TelegramServices.Implementations;

public class TelegramService : ITelegramService
{
    private readonly ITelegramBotClient _botClient;

    private readonly IOptions<TelegramOptions> _telegramOptions;

    public TelegramService(ITelegramBotClient botClient, IOptions<TelegramOptions> telegramOptions)
    {
        _botClient = botClient;
        _telegramOptions = telegramOptions;
    }

    public async Task Send(SendMessageModel message, CancellationToken cancellationToken = default)
    {
        if (!message.EclipseToken.Equals(_telegramOptions.Value.EclipseToken))
        {
            return;
        }

        await _botClient.SendTextMessageAsync(
            message.ChatId,
            message.Message,
            cancellationToken: cancellationToken);
    }
}
