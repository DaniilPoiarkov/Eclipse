using Telegram.Bot;
using Telegram.Bot.Polling;

namespace Eclipse.WebAPI.Services.TelegramServices.Implementations;

public class EclipseStarter : IEclipseStarter
{
    private readonly ITelegramBotClient _client;

    private readonly ILogger<EclipseStarter> _logger;

    private readonly IUpdateHandler _updateHandler;

    public EclipseStarter(ITelegramBotClient botClient, ILogger<EclipseStarter> logger, IUpdateHandler updateHandler)
    {
        _client = botClient;
        _logger = logger;
        _updateHandler = updateHandler;
    }

    public async Task StartAsync()
    {
        _client.StartReceiving(_updateHandler);
        var eclipse = await _client.GetMeAsync();

        _logger.LogInformation("Bot: {token}", eclipse.Username);
    }
}
