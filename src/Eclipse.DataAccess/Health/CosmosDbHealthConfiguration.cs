using Eclipse.DataAccess.Cosmos;

using HealthChecks.CosmosDb;

using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Eclipse.DataAccess.Health;

internal static class CosmosDbHealthConfiguration
{
    internal static IServiceCollection AddDataAccessHealthChecks(this IServiceCollection services)
    {
        services.AddHealthChecks()
            .AddAzureCosmosDB(sp => sp.GetRequiredService<CosmosClient>(), sp => new AzureCosmosDbHealthCheckOptions
            {
                DatabaseId = GetOptions(sp).DatabaseId,
                ContainerIds = [GetOptions(sp).Container]
            }, tags: ["database"]);

        return services;
    }

    private static CosmosDbContextOptions GetOptions(IServiceProvider serviceProvider)
    {
        return serviceProvider.GetRequiredService<IOptions<CosmosDbContextOptions>>().Value;
    }
}
