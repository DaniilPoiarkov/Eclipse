using Eclipse.DataAccess.CosmosDb;

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
            .AddAzureCosmosDB(GetCosmosClient, sp => new AzureCosmosDbHealthCheckOptions
            {
                DatabaseId = sp.GetRequiredService<IOptions<CosmosDbContextOptions>>().Value.DatabaseId,
                ContainerIds = ["IdentityUsers"]
            }, tags: ["database"]);

        return services;
    }

    private static CosmosClient GetCosmosClient(IServiceProvider serviceProvider)
    {
        var options = serviceProvider.GetRequiredService<IOptions<CosmosDbContextOptions>>().Value;

        return new CosmosClient(options.ConnectionString);
    }
}
