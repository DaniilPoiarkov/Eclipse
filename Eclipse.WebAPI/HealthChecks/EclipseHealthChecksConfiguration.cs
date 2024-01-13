using Eclipse.DataAccess.CosmosDb;

using HealthChecks.CosmosDb;
using HealthChecks.UI.Client;

using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Eclipse.WebAPI.HealthChecks;

public static class EclipseHealthChecksConfiguration
{
    public static IServiceCollection AddEclipseHealthChecks(this IServiceCollection services)
    {
        services
            .AddHealthChecks()
            .AddAzureCosmosDB(sp => sp.GetRequiredService<CosmosClient>(), sp => new AzureCosmosDbHealthCheckOptions
            {
                DatabaseId = sp.GetRequiredService<CosmosDbContextOptions>().DatabaseId,
                ContainerIds = ["IdentityUsers"]
            }, tags: ["database"])
            .AddCheck<BotHealthCheck>("eclipse", HealthStatus.Unhealthy, ["telegram-bot"]);

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
