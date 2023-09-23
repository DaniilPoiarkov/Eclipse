using Eclipse.Infrastructure.Telegram;
using Telegram.Bot.Polling;
using Telegram.Bot;
using Serilog;
using Eclipse.Infrastructure.Exceptions;
using Telegram.Bot.Types.Enums;

namespace Eclipse.Infrastructure.Internals.Telegram;

internal class EclipseStarter : IEclipseStarter
{
    private readonly ITelegramBotClient _client;

    private readonly ILogger _logger;

    private readonly IUpdateHandler _updateHandler;

    public bool IsStarted { get; private set; }

    public EclipseStarter(ITelegramBotClient botClient, ILogger logger, IUpdateHandler updateHandler)
    {
        _client = botClient;
        _logger = logger;
        _updateHandler = updateHandler;
    }

    public async Task StartAsync()
    {
        if (IsStarted)
        {
            throw new BotAlreadyRunningException();
        }

        _client.StartReceiving(_updateHandler);
        var eclipse = await _client.GetMeAsync();

        _logger.Information("Bot: {token}", eclipse.Username);

        IsStarted = true;
    }
}
