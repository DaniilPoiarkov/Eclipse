using Eclipse.Common.Cache;
using Eclipse.Common.EventBus;
using Eclipse.Common.Sheets;
using Eclipse.Common.Telegram;
using Eclipse.Infrastructure.Builder;
using Eclipse.Infrastructure.Cache;
using Eclipse.Infrastructure.EventBus;
using Eclipse.Infrastructure.Google;

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
    public static IInfrastructureModuleBuilder AddInfrastructureModule(this IServiceCollection services)
    {
        services
            .AddSerilogIntegration()
            .AddMemoryCacheIntegration()
            .AddTelegramIntegration()
            .AddQuartzIntegration()
            .AddGoogleIntegration();

        services
            .AddSingleton(typeof(InMemoryQueue<>))
            .AddTransient<IEventBus, InMemoryEventBus>()
            .AddHostedService<InMemoryChannelReadService>();

        return new InfrastructureModuleBuilder(services);
    }

    private static IServiceCollection AddGoogleIntegration(this IServiceCollection services)
    {
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
            cfg.SchedulerName = $"EclipseScheduler_{Guid.NewGuid()}";
        });

        services.AddQuartzHostedService(cfg =>
        {
            cfg.WaitForJobsToComplete = false;
        });

        return services;
    }

    private static IServiceCollection AddTelegramIntegration(this IServiceCollection services)
    {
        services.AddSingleton<ITelegramBotClient>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<TelegramOptions>>().Value;
            return new TelegramBotClient(options.Token);
        });

        return services;
    }

    private static IServiceCollection AddMemoryCacheIntegration(this IServiceCollection services)
    {
        services
            //.AddMemoryCache()
            .AddSingleton<ICacheService, AsyncCacheService>();

        services.AddDistributedMemoryCache(options =>
        {
            
        });

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
