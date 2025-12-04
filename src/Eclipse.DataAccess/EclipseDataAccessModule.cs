using Azure.Identity;

using Eclipse.DataAccess.Cosmos;
using Eclipse.DataAccess.Feedbacks;
using Eclipse.DataAccess.InboxMessages;
using Eclipse.DataAccess.Interceptors;
using Eclipse.DataAccess.Migrations;
using Eclipse.DataAccess.Model;
using Eclipse.DataAccess.MoodRecords;
using Eclipse.DataAccess.OutboxMessages;
using Eclipse.DataAccess.Repositories.Caching;
using Eclipse.DataAccess.Statistics;
using Eclipse.DataAccess.Users;
using Eclipse.Domain.Feedbacks;
using Eclipse.Domain.InboxMessages;
using Eclipse.Domain.MoodRecords;
using Eclipse.Domain.OutboxMessages;
using Eclipse.Domain.Statistics;
using Eclipse.Domain.Users;

using Microsoft.AspNetCore.Builder;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using System.Reflection;

namespace Eclipse.DataAccess;

/// <summary>
/// Responsible for data access
/// </summary>
public static class EclipseDataAccessModule
{
    public static IServiceCollection AddDataAccessModule(this IServiceCollection services)
    {
        services.AddCosmosDb()
            .AddDataAccessHealthChecks();

        services.AddSingleton<IExpressionHashStore, ExpressionHashStore>();

        services
            .AddScoped<IUserRepository, UserRepository>()
            .AddScoped<IOutboxMessageRepository, OutboxMessageRepository>()
            .AddScoped<IInboxMessageRepository, InboxMessageRepository>()
            .AddScoped<IMoodRecordRepository, MoodRecordRepository>()
            .AddScoped<IUserStatisticsRepository, UserStatisticsRepository>()
            .AddScoped<IFeedbackRepository, FeedbackRepository>()
            .AddTransient<IInterceptor, DomainEventsToOutboxMessagesInterceptor>();

        services
            .Decorate<IUserRepository, CachedUserRepository>()
            .Decorate<IOutboxMessageRepository, CachedOutboxMessageRepository>()
            .Decorate<IInboxMessageRepository, CachedInboxMessageRepository>()
            .Decorate<IMoodRecordRepository, CachedMoodRecordRepository>()
            .Decorate<IUserStatisticsRepository, CachedUserStatisticsRepository>()
            .Decorate<IFeedbackRepository, CachedFeedbackRepository>();

        services.AddSingleton<IModelBuilderConfigurator, ModelBuilderConfigurator>();

        services.Scan(tss => tss.FromAssemblies(typeof(EclipseDataAccessModule).Assembly)
            .AddClasses(c => c.AssignableTo(typeof(IEntityTypeConfiguration<>)), publicOnly: false)
            .AsImplementedInterfaces()
            .WithSingletonLifetime());

        return services;
    }

    public static IServiceCollection ApplyMigrations(this IServiceCollection services, Assembly assembly)
    {
        services.AddScoped<IMigrationRunner, MigrationRunner<EclipseDbContext>>();

        services.Scan(tss => tss.FromAssemblies(assembly)
            .AddClasses(c => c.AssignableTo<IMigration>(), false)
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        services.AddHostedService<MigrationHostedService>();

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

        services.AddScoped(sp => new CosmosClient(
            sp.GetRequiredService<IOptions<CosmosDbContextOptions>>().Value.Endpoint,
            new DefaultAzureCredential())
        );

        return services;
    }

    private static IServiceCollection AddEmulator(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Emulator")!;
        var databaseId = configuration["Azure:CosmosOptions:DatabaseId"]!;

        services.AddDbContext<EclipseDbContext>((sp, b) =>
            b.UseCosmos(connectionString, databaseId)
                .AddInterceptors(sp.GetServices<IInterceptor>()));

        services.AddSingleton(new CosmosClient(connectionString));

        return services;
    }

    public static async Task InitializeDataAccessModuleAsync(this WebApplication app, CancellationToken cancellationToken = default)
    {
        using var scope = app.Services.CreateScope();

        var serviceProvider = scope.ServiceProvider;

        var configuration = serviceProvider.GetRequiredService<IConfiguration>();
        var logger = serviceProvider.GetRequiredService<ILogger<EclipseDbContext>>();

        if (!configuration.GetValue<bool>("Settings:IsDocker"))
        {
            logger.LogInformation("Initializing {module} is available only for docker environment.", nameof(EclipseDataAccessModule));
            return;
        }

        var options = serviceProvider.GetRequiredService<IOptions<CosmosDbContextOptions>>();

        using var client = new CosmosClient(configuration.GetConnectionString("Emulator"));

        logger.LogInformation("Creating database if it not exists...");
        var database = await client.CreateDatabaseIfNotExistsAsync(options.Value.DatabaseId, ThroughputProperties.CreateManualThroughput(1000), cancellationToken: cancellationToken);

        logger.LogInformation("Creating container if it not exists...");
        await database.Database.CreateContainerIfNotExistsAsync(new ContainerProperties
        {
            Id = options.Value.Container,
            PartitionKeyPath = "/Id",
        }, cancellationToken: cancellationToken);

        logger.LogInformation("{module} module initialized successfully", nameof(EclipseDataAccessModule));
    }
}
