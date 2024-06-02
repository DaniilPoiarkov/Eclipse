using Eclipse.DataAccess.Builder;
using Eclipse.DataAccess.CosmosDb;
using Eclipse.DataAccess.EclipseCosmosDb;
using Eclipse.DataAccess.Health;
using Eclipse.DataAccess.Users;
using Eclipse.DataAccess.Interceptors;
using Eclipse.Domain.Users;

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
            .AddScoped<IUserRepository, UserRepository>()
                .AddTransient<IInterceptor, TriggerDomainEventsInterceptor>();

        services.Configure(builder);

        services.AddCosmosDb()
            .AddDataAccessHealthChecks();

        return services;
    }

    private static IServiceCollection AddCosmosDb(this IServiceCollection services)
    {
        var configuration = services.GetConfiguration();

        services.AddOptions<CosmosDbContextOptions>()
            .BindConfiguration("Azure:CosmosOptions")
            .ValidateOnStart();

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
}
