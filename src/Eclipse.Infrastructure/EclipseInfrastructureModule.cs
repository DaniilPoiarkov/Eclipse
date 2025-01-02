using Eclipse.Common.Background;
using Eclipse.Common.Caching;
using Eclipse.Common.Clock;
using Eclipse.Common.Excel;
using Eclipse.Common.Plots;
using Eclipse.Common.Sheets;
using Eclipse.Infrastructure.Background;
using Eclipse.Infrastructure.Caching;
using Eclipse.Infrastructure.Excel;
using Eclipse.Infrastructure.Google;
using Eclipse.Infrastructure.Plots;

using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Quartz;
using Quartz.Logging;

using Serilog;

namespace Eclipse.Infrastructure;

/// <summary>
/// Takes responsibility for 3rd party services integration and easy to use wrappers around them
/// </summary>
public static class EclipseInfrastructureModule
{
    public static IServiceCollection AddInfrastructureModule(this IServiceCollection services)
    {
#pragma warning disable EXTEXP0018 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
        services
            .AddSerilogIntegration()
            .AddCache()
            .AddQuartzIntegration()
            .AddGoogleIntegration()
            .AddHybridCache();
#pragma warning restore EXTEXP0018 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

        services
            .AddSingleton<IExcelManager, ExcelManager>()
            .AddSingleton<IBackgroundJobManager, BackgroundJobManager>()
            .AddSingleton<IPlotGenerator, PlotGenerator>();

        services.AddSingleton<ITimeProvider, UtcNowTimeProvider>();

        return services;
    }

    private static IServiceCollection AddGoogleIntegration(this IServiceCollection services)
    {
        var configuration = services.GetConfiguration();

        if (!configuration.GetValue<bool>("Settings:IsGoogleEnabled"))
        {
            return services
                .AddSingleton<ISheetsService, NullSheetsService>();
        }

        services.AddOptions<GoogleOptions>()
            .BindConfiguration("Google")
            .ValidateOnStart();

        services
            .AddSingleton<IGoogleClient, GoogleClient>()
            .AddSingleton(sp => sp.GetRequiredService<IGoogleClient>().GetSheetsService())
                .AddScoped<ISheetsService, GoogleSheetsService>();

        return services;
    }

    private static IServiceCollection AddQuartzIntegration(this IServiceCollection services)
    {
        LogProvider.IsDisabled = true;

        services.AddQuartz(configurator =>
        {
            configurator.SchedulerName = $"eclipse";
        });

        services.AddQuartzHostedService(options =>
        {
            options.WaitForJobsToComplete = true;
        });

        return services;
    }

    private static IServiceCollection AddCache(this IServiceCollection services)
    {
        var configuration = services.GetConfiguration();

        if (configuration.GetValue<bool>("Settings:IsRedisEnabled"))
        {
            var connectionString = configuration.GetConnectionString("Redis")
                ?? throw new InvalidOperationException("Redis connection string is not provided");

            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = connectionString;
            });
        }
        else
        {
            services.AddDistributedMemoryCache();
        }

        services.AddSingleton<ICacheService, CacheService>();

        return services;
    }

    private static IServiceCollection AddSerilogIntegration(this IServiceCollection services)
    {
        services.AddSerilog((sp, logger) =>
        {
            logger.ReadFrom.Configuration(sp.GetRequiredService<IConfiguration>())
                .WriteTo.Async(sink => sink.Console())
                .WriteTo.Async(sink => sink.ApplicationInsights(
                    sp.GetRequiredService<TelemetryConfiguration>(),
                    TelemetryConverter.Traces))
                .Enrich.FromLogContext();
        });

        return services;
    }
}
