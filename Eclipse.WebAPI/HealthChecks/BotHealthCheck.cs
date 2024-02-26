using Microsoft.Extensions.Diagnostics.HealthChecks;

using Telegram.Bot;

namespace Eclipse.WebAPI.HealthChecks;

public sealed class BotHealthCheck : IHealthCheck
{
    private readonly ITelegramBotClient _botClient;

    public BotHealthCheck(ITelegramBotClient botClient)
    {
        _botClient = botClient;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var me = await _botClient.GetMeAsync(cancellationToken);
        var apiCallResult = await _botClient.TestApiAsync(cancellationToken);

        var data = new Dictionary<string, object>()
        {
            ["Username"] = me?.Username ?? "NULL",
            ["Api call"] = apiCallResult ? "Passed" : "Failed"
        };

        if (me is not null && apiCallResult)
        {
            return HealthCheckResult.Healthy("Bot responding", data);
        }

        return HealthCheckResult.Unhealthy("Bot not responding", data: data);
    }
}
