using Eclipse.Infrastructure.Telegram;

using ILogger = Serilog.ILogger;

namespace Eclipse.WebAPI.Hosted;

public class EclipseBotHostedService : IHostedService
{
    private readonly IEclipseStarter _eclipseStarter;
    private readonly ILogger _logger;

    public EclipseBotHostedService(IEclipseStarter eclipseStarter, ILogger logger)
    {
        _eclipseStarter = eclipseStarter;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.Information("Initialization of {service}", nameof(IEclipseStarter));
        await _eclipseStarter.StartAsync();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
