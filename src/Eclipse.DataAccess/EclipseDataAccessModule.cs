﻿using Azure.Identity;

using Eclipse.DataAccess.CosmosDb;
using Eclipse.DataAccess.EclipseCosmosDb;
using Eclipse.DataAccess.Health;
using Eclipse.DataAccess.Interceptors;
using Eclipse.DataAccess.Repositories;
using Eclipse.DataAccess.Users;
using Eclipse.Domain.Shared.Repositories;
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
    public static IServiceCollection AddDataAccessModule(this IServiceCollection services)
    {
        services
            .AddScoped(typeof(IRepository<>), typeof(RepositoryBase<>))
            .AddScoped<IUserRepository, UserRepository>()
                .AddTransient<IInterceptor, TriggerDomainEventsInterceptor>();

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

            builder
                .UseCosmos(
                    options.Endpoint,
                    new DefaultAzureCredential(),
                    options.DatabaseId)
                .AddInterceptors(interceptors);
        });

        return services;
    }
}
