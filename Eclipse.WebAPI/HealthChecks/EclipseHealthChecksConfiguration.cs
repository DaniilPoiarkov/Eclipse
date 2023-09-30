using HealthChecks.UI.Client;

using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Eclipse.WebAPI.HealthChecks;

public static class EclipseHealthChecksConfiguration
{
    public static IServiceCollection AddEclipseHealthChecks(this IServiceCollection services)
    {
        services.AddHealthChecks()
            .AddCheck<BotHealthCheck>("eclipse", HealthStatus.Unhealthy, new[] { "telegram-bot" });

        return services;
    }

    public static IApplicationBuilder UseEclipseHealthCheks(this IApplicationBuilder app)
    {
        var options = new HealthCheckOptions
        {
            AllowCachingResponses = true,
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        };

        app.UseHealthChecks(new PathString("/_health-checks"), options);

        return app;
    }
}
