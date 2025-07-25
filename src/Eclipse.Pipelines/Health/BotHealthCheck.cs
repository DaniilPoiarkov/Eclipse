﻿using Microsoft.Extensions.Diagnostics.HealthChecks;

using Telegram.Bot;

namespace Eclipse.Pipelines.Health;

public sealed class BotHealthCheck : IHealthCheck
{
    private readonly ITelegramBotClient _botClient;

    public BotHealthCheck(ITelegramBotClient botClient)
    {
        _botClient = botClient;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var me = await _botClient.GetMe(cancellationToken);
        var apiCallResult = await _botClient.TestApi(cancellationToken);

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
