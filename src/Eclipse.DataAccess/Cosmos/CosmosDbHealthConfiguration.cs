using HealthChecks.CosmosDb;

using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Eclipse.DataAccess.Cosmos;

internal static class CosmosDbHealthConfiguration
{
    internal static IServiceCollection AddDataAccessHealthChecks(this IServiceCollection services)
    {
        services.AddHealthChecks()
            .AddAzureCosmosDB(sp => sp.GetRequiredService<CosmosClient>(), GetHealthCheckOptions, tags: ["database"]);

        return services;
    }

    private static AzureCosmosDbHealthCheckOptions GetHealthCheckOptions(IServiceProvider serviceProvider)
    {
        var options = serviceProvider.GetRequiredService<IOptions<CosmosDbContextOptions>>();

        return new AzureCosmosDbHealthCheckOptions
        {
            DatabaseId = options.Value.DatabaseId,
            ContainerIds = [options.Value.Container]
        };
    }
}
