using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Eclipse.WebAPI.HealthChecks;

public static class EclipseHealthChecksConfiguration
{
    public static IServiceCollection AddEclipseHealthChecks(this IServiceCollection services)
    {
        services.AddHealthChecks()
            .AddCheck<BotHealthCheck>("starter", HealthStatus.Unhealthy, new[] { "telegram-bot" });

        return services;
    }

    public static IApplicationBuilder UseEclipseHealthCheks(this IApplicationBuilder app)
    {
        var options = new HealthCheckOptions
        {
            AllowCachingResponses = true,
            Predicate = _ => true
        };

        app.UseHealthChecks(new PathString("/health-checks"), options);

        return app;
    }
}
