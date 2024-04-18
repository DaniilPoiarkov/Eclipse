using Eclipse.DataAccess.Builder;
using Eclipse.DataAccess.CosmosDb;
using Eclipse.DataAccess.EclipseCosmosDb;
using Eclipse.DataAccess.Health;
using Eclipse.DataAccess.IdentityUsers;
using Eclipse.DataAccess.Interceptors;
using Eclipse.Domain.IdentityUsers;

using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
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
            .AddScoped<IIdentityUserRepository, EFCoreIdentityUserRepository>()
                .AddTransient<IInterceptor, TriggerDomainEventsInterceptor>();

        services.Configure(builder);

        services.AddCosmosDb()
            .AddDataAccessHealthChecks();

        return services;
    }

    private static IServiceCollection AddCosmosDb(this IServiceCollection services)
    {
        var configuration = services.GetConfiguration();

        services.Configure<CosmosDbContextOptions>(
            configuration.GetSection("Azure:CosmosOptions")
        );

        services.AddDbContextFactory<EclipseDbContext>((sp, builder) =>
        {
            var options = sp.GetRequiredService<IOptions<CosmosDbContextOptions>>().Value;
            var interceptors = sp.GetServices<IInterceptor>();
            
            // TODO: Use RBAC instead of connection string
            builder
                .UseCosmos(
                    options.ConnectionString,
                    options.DatabaseId
                )
                .AddInterceptors(interceptors);
        });

        return services;
    }

    public static async Task InitializeDataAccessModule(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var serviceProvider = scope.ServiceProvider;

        var logger = serviceProvider.GetRequiredService<Serilog.ILogger>();
        var context = serviceProvider.GetRequiredService<EclipseDbContext>();

        logger.Information("Initializing {module} module", nameof(EclipseDataAccessModule));

        logger.Information("\tInitializing database");

        await context.Database.EnsureCreatedAsync();

        logger.Information("{module} module initialized successfully", nameof(EclipseDataAccessModule));
    }
}
