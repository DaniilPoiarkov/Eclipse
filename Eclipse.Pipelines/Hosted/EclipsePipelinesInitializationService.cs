using Eclipse.Infrastructure.Telegram;

using Microsoft.Extensions.Hosting;

using Serilog;

namespace Eclipse.Pipelines.Hosted;

internal class EclipsePipelinesInitializationService : IHostedService
{
    private readonly IEclipseStarter _eclipseStarter;
    private readonly ILogger _logger;

    public EclipsePipelinesInitializationService(IEclipseStarter eclipseStarter, ILogger logger)
    {
        _eclipseStarter = eclipseStarter;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.Information("Initializing {module} module", nameof(EclipsePipelinesModule));

        await _eclipseStarter.StartAsync();

        _logger.Information("{module} module initialized successfully", nameof(EclipsePipelinesModule));
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
