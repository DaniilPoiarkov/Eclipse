using Eclipse.DataAccess.Builder;
using Eclipse.DataAccess.EclipseCosmosDb;
using Eclipse.DataAccess.Hosted;
using Eclipse.DataAccess.InMemoryDb;
using Eclipse.DataAccess.TodoItems;
using Eclipse.Domain.TodoItems;

using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Eclipse.DataAccess;

/// <summary>
/// Responsible for data access
/// </summary>
public static class EclipseDataAccessModule
{
    public static IServiceCollection AddDataAccessModule(this IServiceCollection services, Action<DataAccessModuleBuilder> builder)
    {
        ArgumentNullException.ThrowIfNull(builder, nameof(builder));

        services
            .AddSingleton<IDbContext, InMemoryDbContext>()
            .AddScoped<ITodoItemRepository, CosmosTodoItemRepository>();

        services.AddHostedService<DataAccessModuleInitializationService>();

        services.Configure(builder);

        services.AddCosmosDb();

        return services;
    }

    private static IServiceCollection AddCosmosDb(this IServiceCollection services)
    {
        services.AddSingleton(sp =>
        {
            var options = sp.GetRequiredService<IOptions<DataAccessModuleBuilder>>().Value;

            // TODO: Use Default credentials instead of connection string
            //return new CosmosClient(
            //    accountEndpoint: options.CosmosOptions.Endpoint,
            //    tokenCredential: new DefaultAzureCredential());

            return new CosmosClient(options.CosmosOptions.ConnectionString);
        });

        services.AddSingleton(sp =>
        {
            var options = sp.GetRequiredService<IOptions<DataAccessModuleBuilder>>().Value;
            return options.CosmosOptions;
        });

        services.AddSingleton<EclipseCosmosDbContext>();

        return services;
    }
}
