﻿using HealthChecks.UI.Client;

using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Eclipse.WebAPI.HealthChecks;

public static class EclipseHealthChecksConfiguration
{
    private static readonly string[] tags = ["telegram-bot"];

    public static IServiceCollection AddEclipseHealthChecks(this IServiceCollection services)
    {
        services
            .AddHealthChecks()
            .AddCheck<BotHealthCheck>("eclipse", HealthStatus.Unhealthy, tags);

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
