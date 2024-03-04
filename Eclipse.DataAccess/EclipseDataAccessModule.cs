using Eclipse.DataAccess.Builder;
using Eclipse.DataAccess.EclipseCosmosDb;
using Eclipse.DataAccess.Health;
using Eclipse.DataAccess.IdentityUsers;
using Eclipse.Domain.IdentityUsers;

using Microsoft.AspNetCore.Builder;
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
            .AddScoped<IIdentityUserRepository, IdentityUserRepository>();

        services.Configure(builder);

        services.AddCosmosDb()
            .AddDataAccessHealthChecks();

        return services;
    }

    private static IServiceCollection AddCosmosDb(this IServiceCollection services)
    {
        services.AddSingleton(sp =>
        {
            var options = sp.GetRequiredService<IOptions<DataAccessModuleBuilder>>().Value;

            // TODO: Use RBAC instead of connection string
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

    public static async Task InitializeDataAccessModule(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var serviceProvider = scope.ServiceProvider;

        var logger = serviceProvider.GetRequiredService<Serilog.ILogger>();
        var context = serviceProvider.GetRequiredService<EclipseCosmosDbContext>();

        logger.Information("Initializing {module} module", nameof(EclipseDataAccessModule));

        logger.Information("\tInitializing database");
        await context.InitializeAsync();

        logger.Information("{module} module initialized successfully", nameof(EclipseDataAccessModule));
    }
}
