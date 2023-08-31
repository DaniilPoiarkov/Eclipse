using Eclipse.WebAPI.Services.Cache;
using Eclipse.WebAPI.Services.Cache.Implementations;
using Eclipse.WebAPI.Services.TelegramServices;
using Eclipse.WebAPI.Services.TelegramServices.Implementations;
using Telegram.Bot;
using Telegram.Bot.Polling;

namespace Eclipse.WebAPI.Helpers;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.Scan(tss =>
            tss.FromAssemblyOf<WeatherForecast>()
                .AddClasses()
                .AsMatchingInterface()
                .WithTransientLifetime());

        services
            .AddTransient<IUpdateHandler, TelegramUpdateHandler>()
            .AddSingleton<ICacheService, CacheService>();
        
        return services;
    }

    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddControllers();

        services
            .AddEndpointsApiExplorer()
            .AddSwaggerGen()
            .AddMemoryCache();

        return services;
    }

    public static ITelegramBotClient AddEclipseBot(this WebApplicationBuilder builder)
    {
        var telegramConfig = builder.Configuration.GetSection("Telegram");

        builder.Services.Configure<TelegramOptions>(options =>
        {
            options.Token = telegramConfig["Token"]!;
            options.EclipseToken = telegramConfig["EclipseToken"]!;
        });

        var client = new TelegramBotClient(telegramConfig["Token"]!);

        builder.Services.AddSingleton<ITelegramBotClient>(client);

        return client;
    }
}
