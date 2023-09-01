using Eclipse.Infrastructure.Telegram;
using Telegram.Bot.Polling;
using Telegram.Bot;
using Serilog;

namespace Eclipse.Infrastructure.Internals.Telegram;

internal class EclipseStarter : IEclipseStarter
{
    private readonly ITelegramBotClient _client;

    private readonly ILogger _logger;

    private readonly IUpdateHandler _updateHandler;

    public EclipseStarter(ITelegramBotClient botClient, ILogger logger, IUpdateHandler updateHandler)
    {
        _client = botClient;
        _logger = logger;
        _updateHandler = updateHandler;
    }

    public async Task StartAsync()
    {
        _client.StartReceiving(_updateHandler);
        var eclipse = await _client.GetMeAsync();

        _logger.Information("Bot: {token}", eclipse.Username);
    }
}
