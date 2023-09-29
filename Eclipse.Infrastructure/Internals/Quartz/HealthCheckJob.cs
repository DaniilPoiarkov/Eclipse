using Quartz;

using Serilog;

using Telegram.Bot;

namespace Eclipse.Infrastructure.Internals.Quartz;

internal class HealthCheckJob : IJob
{
    private readonly ITelegramBotClient _botClient;

    private readonly ILogger _logger;

    public HealthCheckJob(ITelegramBotClient botClient, ILogger logger)
    {
        _botClient = botClient;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var me = await _botClient.GetMeAsync(context.CancellationToken);

        if (me is null || !await _botClient.TestApiAsync(context.CancellationToken))
        {
            _logger.Error("Bot not responding. Me is {isNull}", me?.Username ?? "NULL");
        }
    }
}
