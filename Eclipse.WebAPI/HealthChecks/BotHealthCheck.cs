using Microsoft.Extensions.Diagnostics.HealthChecks;

using Telegram.Bot;

namespace Eclipse.WebAPI.HealthChecks;

public class BotHealthCheck : IHealthCheck
{
    private readonly ITelegramBotClient _botClient;

    public BotHealthCheck(ITelegramBotClient botClient)
    {
        _botClient = botClient;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var me = await _botClient.GetMeAsync(cancellationToken);

            if (me is null)
            {
                return HealthCheckResult.Unhealthy("Me is null");
            }

            return HealthCheckResult.Healthy($"Bot me: {me.Username}");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Exception during health-check", ex);
        }
    }
}
