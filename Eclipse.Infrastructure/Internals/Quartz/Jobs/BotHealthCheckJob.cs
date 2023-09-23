using Eclipse.Infrastructure.Telegram;

using Quartz;

using Serilog;

using Telegram.Bot;

namespace Eclipse.Infrastructure.Internals.Quartz.Jobs;

internal class BotHealthCheckJob : IJob
{
    private readonly IEclipseStarter _eclipseStarter;

    private readonly ILogger _logger;

    private readonly ITelegramBotClient _botClient;

    public BotHealthCheckJob(IEclipseStarter eclipseStarter, ILogger logger, ITelegramBotClient botClient)
    {
        _eclipseStarter = eclipseStarter;
        _logger = logger;
        _botClient = botClient;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        if (!_eclipseStarter.IsStarted)
        {
            _logger.Warning("Eclipse state: {state}", _eclipseStarter.IsStarted);
            await _eclipseStarter.StartAsync();
        }

        var me = await _botClient.GetMeAsync(context.CancellationToken);

        _logger.Information("Bot me username: {username}", me.Username);
    }
}
