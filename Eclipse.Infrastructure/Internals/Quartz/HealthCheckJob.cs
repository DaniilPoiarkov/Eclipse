using Quartz;

using Serilog;

using Telegram.Bot;

namespace Eclipse.Infrastructure.Internals.Quartz;

internal class HealthCheckJob : IJob
{
    private readonly ITelegramBotClient _botClient;

    private readonly ILogger _logger;

    private readonly HttpClient _client;

    public HealthCheckJob(ITelegramBotClient botClient, ILogger logger, HttpClient client)
    {
        _botClient = botClient;
        _logger = logger;
        _client = client;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var me = await _botClient.GetMeAsync(context.CancellationToken);

        if (me is null || !await _botClient.TestApiAsync(context.CancellationToken))
        {
            _logger.Error("Bot not responding. Me is {isNull}", me?.Username ?? "NULL");
        }

        var response = await _client.GetAsync("/health-check", context.CancellationToken);
        var content = await response.Content.ReadAsStringAsync(cancellationToken: context.CancellationToken);

        _logger.Information("Health check result: {health}", content);
    }
}
