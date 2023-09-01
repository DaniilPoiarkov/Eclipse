using Eclipse.Infrastructure.Builder;
using Eclipse.Infrastructure.Cache;
using Eclipse.Infrastructure.Internals.Cache;
using Eclipse.Infrastructure.Internals.Telegram;
using Eclipse.Infrastructure.Telegram;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Serilog;
using Telegram.Bot;

namespace Eclipse.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, Action<InfrastructureOptionsBuilder> options)
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

        return services;
    }
}
