﻿using Eclipse.Common.Background;
using Eclipse.Common.Caching;
using Eclipse.Common.Clock;
using Eclipse.Common.EventBus;
using Eclipse.Common.Excel;
using Eclipse.Common.Sheets;
using Eclipse.Common.Telegram;
using Eclipse.Infrastructure.Background;
using Eclipse.Infrastructure.Caching;
using Eclipse.Infrastructure.EventBus;
using Eclipse.Infrastructure.Excel;
using Eclipse.Infrastructure.Google;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Polly;
using Polly.Contrib.WaitAndRetry;

using Quartz;
using Quartz.Logging;

using Serilog;

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
            .AddTelegramIntegration()
            .AddQuartzIntegration()
            .AddGoogleIntegration();

        services
            .AddSingleton(typeof(InMemoryQueue<>))
            .AddTransient<IEventBus, InMemoryEventBus>()
            .AddHostedService<InMemoryChannelReadService>();

        services
            .AddSingleton<IExcelManager, ExcelManager>()
            .AddSingleton<IBackgroundJobManager, BackgroundJobManager>();

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
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = configuration.GetConnectionString("Redis");
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
        services.AddSerilog((_, configuration) =>
        {
            configuration.WriteTo.Console();
        });

        return services;
    }
}
