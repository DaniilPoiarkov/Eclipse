using Eclipse.Infrastructure.Telegram;

using Microsoft.Extensions.Diagnostics.HealthChecks;

using ILogger = Serilog.ILogger;

namespace Eclipse.WebAPI.HealthChecks;

public class StarterHealthCheck : IHealthCheck
{
    private readonly IEclipseStarter _eclipseStarter;

    private readonly ILogger _logger;

    public StarterHealthCheck(IEclipseStarter eclipseStarter, ILogger logger)
    {
        _eclipseStarter = eclipseStarter;
        _logger = logger;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!_eclipseStarter.IsStarted)
            {
                _logger.Warning("Starter status is {status}", _eclipseStarter.IsStarted);
                await _eclipseStarter.StartAsync();
            }

            return HealthCheckResult.Healthy();
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Exception during starter health-check", ex);
        }
    }
}
