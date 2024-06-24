using Eclipse.Common.Background;
using Eclipse.Common.Cache;
using Eclipse.Common.EventBus;
using Eclipse.Common.Excel;
using Eclipse.Common.Sheets;
using Eclipse.Common.Telegram;
using Eclipse.Infrastructure.Background;
using Eclipse.Infrastructure.Cache;
using Eclipse.Infrastructure.EventBus;
using Eclipse.Infrastructure.Excel;
using Eclipse.Infrastructure.Google;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

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

        services.AddQuartz(cfg =>
        {
            cfg.InterruptJobsOnShutdown = true;
            cfg.SchedulerName = $"eclipse";
        });

        services.AddQuartzHostedService(cfg =>
        {
            cfg.WaitForJobsToComplete = false;
        });

        return services;
    }

    private static IServiceCollection AddTelegramIntegration(this IServiceCollection services)
    {
        services.AddOptions<TelegramOptions>()
            .BindConfiguration("Telegram")
            .ValidateOnStart();

        services.AddSingleton<ITelegramBotClient>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<TelegramOptions>>().Value;
            return new TelegramBotClient(options.Token);
        });

        return services;
    }

    private static IServiceCollection AddCache(this IServiceCollection services)
    {
        var configuration = services.GetConfiguration();

        if (configuration.GetValue<bool>("IsRedisEnabled"))
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
