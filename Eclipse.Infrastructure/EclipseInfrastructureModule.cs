using Eclipse.Core;
using Eclipse.Infrastructure.Builder;
using Eclipse.Infrastructure.Cache;
using Eclipse.Infrastructure.Internals.Cache;
using Eclipse.Infrastructure.Internals.Telegram;
using Eclipse.Infrastructure.Quartz;
using Eclipse.Infrastructure.Quartz.Jobs;
using Eclipse.Infrastructure.Telegram;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Quartz;
using Serilog;
using Telegram.Bot;

namespace Eclipse.Infrastructure;

/// <summary>
/// Takes responsibility for 3rd party services integration and easy to use wrapprers around them
/// </summary>
public static class EclipseInfrastructureModule
{
    public static IServiceCollection AddInfrastructureModule(this IServiceCollection services, Action<InfrastructureOptionsBuilder> options)
    {
        var builder = new InfrastructureOptionsBuilder(services);

        options(builder);

        var config = builder.Build();

        services.AddSingleton(config)
            .AddMemoryCache()
            .AddSerilog((_, configuration) =>
            {
                configuration.WriteTo.Console();
            });

        services.TryAddSingleton<ICacheService, CacheService>();

        services.TryAddTransient<IEclipseStarter, EclipseStarter>();
        services.TryAddTransient<ITelegramService, TelegramService>();
        
        services.TryAddTransient<ITelegramBotClient>(sp =>
        {
            var options = sp.GetRequiredService<InfrastructureOptions>();

            return new TelegramBotClient(options.TelegramOptions.Token);
        });

        services.AddHttpClient<WarmupJob>((sp, client) =>
        {
            var config = sp.GetRequiredService<IConfiguration>();
            client.BaseAddress = new(config["App:SelfUrl"]!);
        });

        services.AddQuartz();
        services.AddQuartzHostedService(cfg =>
        {
            cfg.WaitForJobsToComplete = true;
        });

        services.ConfigureOptions<QuartzOptionsConfiguration>();

        return services;
    }
}
