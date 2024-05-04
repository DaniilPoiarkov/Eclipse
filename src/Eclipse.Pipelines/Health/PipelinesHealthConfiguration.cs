using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Eclipse.Pipelines.Health;

internal static class PipelinesHealthConfiguration
{
    public static IServiceCollection AddPipelinesHealthChecks(this IServiceCollection services)
    {
        services
            .AddHealthChecks()
            .AddCheck<BotHealthCheck>("eclipse", HealthStatus.Unhealthy, ["telegram-bot"]);

        return services;
    }
}
