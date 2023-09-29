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
        if (!await _botClient.TestApiAsync(cancellationToken))
        {
            return HealthCheckResult.Unhealthy("Bot API Token is invalid");
        }

        return HealthCheckResult.Healthy();
    }
}
