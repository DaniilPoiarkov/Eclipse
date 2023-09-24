using Eclipse.Infrastructure.Builder;
using Eclipse.Infrastructure.Cache;
using Eclipse.Infrastructure.Google;
using Eclipse.Infrastructure.Google.Sheets;
using Eclipse.Infrastructure.Quartz;
using Eclipse.Infrastructure.Telegram;
using Eclipse.Infrastructure.Internals.Cache;
using Eclipse.Infrastructure.Internals.Google;
using Eclipse.Infrastructure.Internals.Google.Sheets;
using Eclipse.Infrastructure.Internals.Telegram;
using Eclipse.Infrastructure.Internals.Quartz.Jobs;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using Quartz;
using Quartz.Logging;

using Serilog;

using Telegram.Bot;

using Polly;
using Polly.Contrib.WaitAndRetry;
using Eclipse.Infrastructure.Internals.Quartz;

namespace Eclipse.Infrastructure;

/// <summary>
/// Takes responsibility for 3rd party services integration and easy to use wrappers around them
/// </summary>
public static class EclipseInfrastructureModule
{
    private static readonly int _baseDelay = 1;

    private static readonly int _retryCount = 5;

    public static IServiceCollection AddInfrastructureModule(this IServiceCollection services, Action<InfrastructureOptionsBuilder> options)
    {
        var builder = new InfrastructureOptionsBuilder(services);

        options(builder);

        var config = builder.Build();

        services.AddSingleton(config);

        services
            .AddSerilogIntegration()
            .AddMemoryCacheIntegration()
            .AddTelegramIntegration()
            .AddQuartzIntegration()
            .AddGoogleIntegration();

        return services;
    }

    private static IServiceCollection AddGoogleIntegration(this IServiceCollection services)
    {
        services.AddSingleton<IGoogleClient>(sp =>
        {
            var options = sp.GetRequiredService<InfrastructureOptions>().GoogleOptions;
            return new GoogleClient(options.Credentials);
        });

        services.AddSingleton(sp => sp.GetRequiredService<IGoogleClient>().GetSheetsService());

        services.AddScoped<IGoogleSheetsService, GoogleSheetsService>();

        return services;
    }

    private static IServiceCollection AddQuartzIntegration(this IServiceCollection services)
    {
        LogProvider.IsDisabled = true;

        services.ConfigureOptions<QuartzOptionsConfiguration>();

        services.AddHttpClient<HealthCheckJob>((sp, client) =>
        {
            var config = sp.GetRequiredService<IConfiguration>();
            client.BaseAddress = new(config["App:SelfUrl"]!);
        }).AddTransientHttpErrorPolicy(pb => 
            pb.WaitAndRetryAsync(
                Backoff.DecorrelatedJitterBackoffV2(TimeSpan.FromSeconds(_baseDelay), _retryCount)
            ));

        services.AddQuartz(cfg =>
        {
            cfg.InterruptJobsOnShutdown = true;
            cfg.SchedulerName = "EclipseScheduler";
        });

        services.AddQuartzHostedService(cfg =>
        {
            cfg.WaitForJobsToComplete = true;
        });

        services.AddTransient<IEclipseScheduler, EclipseScheduler>();

        return services;
    }

    private static IServiceCollection AddTelegramIntegration(this IServiceCollection services)
    {
        services.AddSingleton<IEclipseStarter, EclipseStarter>();

        services.AddSingleton<ITelegramBotClient>(sp =>
        {
            var options = sp.GetRequiredService<InfrastructureOptions>();
            return new TelegramBotClient(options.TelegramOptions.Token);
        });

        return services;
    }

    private static IServiceCollection AddMemoryCacheIntegration(this IServiceCollection services)
    {
        services.AddMemoryCache()
            .TryAddSingleton<ICacheService, CacheService>();

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
