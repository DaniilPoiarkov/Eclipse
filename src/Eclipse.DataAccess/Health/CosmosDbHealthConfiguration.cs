using Azure.Identity;

using Eclipse.DataAccess.CosmosDb;

using HealthChecks.CosmosDb;

using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
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
                DatabaseId = GetOptions(sp).DatabaseId,
                ContainerIds = [GetOptions(sp).Container]
            }, tags: ["database"]);

        return services;
    }

    private static CosmosClient GetCosmosClient(IServiceProvider serviceProvider)
    {
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();

        if (configuration.GetValue<bool>("Settings:IsDocker"))
        {
            return new CosmosClient(configuration.GetConnectionString("Emulator"));
        }

        var options = GetOptions(serviceProvider);

        return new CosmosClient(options.Endpoint, new DefaultAzureCredential());
    }

    private static CosmosDbContextOptions GetOptions(IServiceProvider serviceProvider)
    {
        return serviceProvider.GetRequiredService<IOptions<CosmosDbContextOptions>>().Value;
    }
}
