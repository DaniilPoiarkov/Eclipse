using Eclipse.DataAccess.CosmosDb;

using HealthChecks.CosmosDb;

using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.DependencyInjection;

namespace Eclipse.DataAccess.Health;

internal static class CosmosDbHealthConfiguration
{
    internal static IServiceCollection AddDataAccessHealthChecks(this IServiceCollection services)
    {
        services.AddHealthChecks()
            .AddAzureCosmosDB(sp => sp.GetRequiredService<CosmosClient>(), sp => new AzureCosmosDbHealthCheckOptions
            {
                DatabaseId = sp.GetRequiredService<CosmosDbContextOptions>().DatabaseId,
                ContainerIds = ["IdentityUsers"]
            }, tags: ["database"]);

        return services;
    }
}
