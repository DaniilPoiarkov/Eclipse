using Azure.Identity;

using Eclipse.DataAccess.Constants;
using Eclipse.DataAccess.CosmosDb;
using Eclipse.DataAccess.Health;
using Eclipse.DataAccess.Interceptors;
using Eclipse.DataAccess.OutboxMessages;
using Eclipse.DataAccess.Users;
using Eclipse.Domain.OutboxMessages;
using Eclipse.Domain.Users;

using Microsoft.AspNetCore.Builder;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Eclipse.DataAccess;

/// <summary>
/// Responsible for data access
/// </summary>
public static class EclipseDataAccessModule
{
    public static IServiceCollection AddDataAccessModule(this IServiceCollection services)
    {
        services
            .AddScoped<IUserRepository, UserRepository>()
            .AddScoped<IOutboxMessageRepository, OutboxMessageRepository>()
            .AddTransient<IInterceptor, DomainEventsToOutboxMessagesInterceptor>();

        services.AddCosmosDb()
            .AddDataAccessHealthChecks();

        services
            .Decorate<IUserRepository, CachedUserRepository>()
            .Decorate<IOutboxMessageRepository, CachedOutboxMessageRepository>();

        return services;
    }

    private static IServiceCollection AddCosmosDb(this IServiceCollection services)
    {
        var configuration = services.GetConfiguration();

        services.AddOptions<CosmosDbContextOptions>()
            .BindConfiguration("Azure:CosmosOptions")
            .ValidateOnStart();

        // As cosmosdb emulator doesn't support RBAC access we providing connection string in docker-compose file
        if (configuration.GetValue<bool>("Settings:IsDocker"))
        {
            services.AddEmulator(configuration);
        }
        else
        {
            services.AddAzureInstance();
        }

        return services;
    }

    private static IServiceCollection AddAzureInstance(this IServiceCollection services)
    {
        services.AddDbContextFactory<EclipseDbContext>((sp, builder) =>
        {
            var options = sp.GetRequiredService<IOptions<CosmosDbContextOptions>>().Value;
            var interceptors = sp.GetServices<IInterceptor>();

            builder
                .UseCosmos(
                    options.Endpoint,
                    new DefaultAzureCredential(),
                    options.DatabaseId)
                .AddInterceptors(interceptors);
        });

        return services;
    }

    private static IServiceCollection AddEmulator(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<EclipseDbContext>((sp, b) =>
            b.UseCosmos(configuration.GetConnectionString("Emulator")!, configuration["Azure:CosmosOptions:DatabaseId"]!)
                .AddInterceptors(sp.GetServices<IInterceptor>()));

        return services;
    }

    public static async Task InitializaDataAccessModuleAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<EclipseDbContext>>();

        if (!configuration.GetValue<bool>("Settings:IsDocker"))
        {
            logger.LogInformation("Initializing {module} is available only for docker environment.", nameof(EclipseDataAccessModule));
            return;
        }

        var cosmosOptions = scope.ServiceProvider.GetRequiredService<IOptions<CosmosDbContextOptions>>();

        using var client = new CosmosClient(configuration.GetConnectionString("Emulator"));

        logger.LogInformation("Creating database if it not exists...");
        var database = await client.CreateDatabaseIfNotExistsAsync(cosmosOptions.Value.DatabaseId, ThroughputProperties.CreateManualThroughput(1000));

        logger.LogInformation("Creating container if it not exists...");
        await database.Database.CreateContainerIfNotExistsAsync(new ContainerProperties
        {
            Id = ContainerNames.Aggregates,
            PartitionKeyPath = "/Id",
        });

        logger.LogInformation("{module} module initialized successfully", nameof(EclipseDataAccessModule));
    }
}
