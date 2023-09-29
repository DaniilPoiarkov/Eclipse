using Microsoft.Extensions.Hosting;

using Serilog;

using Telegram.Bot;
using Telegram.Bot.Polling;

namespace Eclipse.Pipelines.Hosted;

internal class EclipsePipelinesInitializationService : IHostedService
{
    private readonly IUpdateHandler _updateHandler;

    private readonly ITelegramBotClient _client;

    private readonly ILogger _logger;

    public EclipsePipelinesInitializationService(ILogger logger, IUpdateHandler updateHandler, ITelegramBotClient client)
    {
        _logger = logger;
        _updateHandler = updateHandler;
        _client = client;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.Information("Initializing {module} module", nameof(EclipsePipelinesModule));

        _client.StartReceiving(_updateHandler, cancellationToken: cancellationToken);

        var me = await _client.GetMeAsync(cancellationToken);

        _logger.Information("\tBot: {bot}", me.Username);
        _logger.Information("{module} module initialized successfully", nameof(EclipsePipelinesModule));
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
