using Eclipse.Common.Background;
using Eclipse.Common.Caching;
using Eclipse.Common.Clock;
using Eclipse.Common.EventBus;
using Eclipse.Common.Excel;
using Eclipse.Common.Plots;
using Eclipse.Common.Sheets;
using Eclipse.Common.Telegram;
using Eclipse.Infrastructure.Background;
using Eclipse.Infrastructure.Caching;
using Eclipse.Infrastructure.EventBus.InMemory;
using Eclipse.Infrastructure.EventBus.Redis;
using Eclipse.Infrastructure.Excel;
using Eclipse.Infrastructure.Google;
using Eclipse.Infrastructure.Plots;

using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Polly;
using Polly.Contrib.WaitAndRetry;

using Quartz;
using Quartz.Logging;

using Serilog;

using StackExchange.Redis;

using Telegram.Bot;

namespace Eclipse.Infrastructure;

/// <summary>
/// Takes responsibility for 3rd party services integration and easy to use wrappers around them
/// </summary>
public static class EclipseInfrastructureModule
{
    private static readonly int _retriesCount = 5;

    private static readonly int _baseRetryDelay = 1;

    public static IServiceCollection AddInfrastructureModule(this IServiceCollection services)
    {
        services
            .AddSerilogIntegration()
            .AddCache()
            .AddEventBus()
            .AddTelegramIntegration()
            .AddQuartzIntegration()
            .AddGoogleIntegration();

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
            configurator.InterruptJobsOnShutdown = true;
            configurator.SchedulerName = $"eclipse";
        });

        services.AddQuartzHostedService(options =>
        {
            options.WaitForJobsToComplete = true;
        });

        return services;
    }

    private static IServiceCollection AddTelegramIntegration(this IServiceCollection services)
    {
        services.AddOptions<TelegramOptions>()
            .BindConfiguration("Telegram")
            .ValidateOnStart();

        services.AddHttpClient<ITelegramBotClient, TelegramBotClient>((client, sp) =>
        {
            var options = sp.GetRequiredService<IOptions<TelegramOptions>>().Value;
            return new TelegramBotClient(options.Token, client);
        }).AddTransientHttpErrorPolicy(policy => policy.WaitAndRetryAsync(
                Backoff.DecorrelatedJitterBackoffV2(TimeSpan.FromSeconds(_baseRetryDelay), _retriesCount)
            ));

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

    private static IServiceCollection AddEventBus(this IServiceCollection services)
    {
        var configuration = services.GetConfiguration();

        if (configuration.GetValue<bool>("Settings:IsRedisEnabled"))
        {
            var connectionString = configuration.GetConnectionString("Redis")
                ?? throw new InvalidOperationException("Redis connection string is not provided");

            services
                .AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(connectionString))
                .AddTransient<IEventBus, RedisEventBus>()
                .AddHostedService<RedisChannelReadService>();

            return services;
        }

        services
            .AddSingleton(typeof(InMemoryQueue<>))
            .AddTransient<IEventBus, InMemoryEventBus>()
            .AddHostedService<InMemoryChannelReadService>();

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
