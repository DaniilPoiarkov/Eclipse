using Eclipse.WebAPI.Filters;
using Eclipse.WebAPI.Services.Cache;
using Eclipse.WebAPI.Services.Cache.Implementations;
using Eclipse.WebAPI.Services.TelegramServices;
using Eclipse.WebAPI.Services.TelegramServices.Implementations;
using Microsoft.OpenApi.Models;
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
            .AddSingleton<ICacheService, CacheService>()
            .AddScoped<ApiKeyAuthorizationAttribute>();
        
        return services;
    }

    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddControllers();

        services
            .AddEndpointsApiExplorer()
            .AddMemoryCache();

        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Eclipse",
                Description = "Open API to test an app. Some features might require special access",
                Version = "v1",
            });

            var apiKeySecurity = "API-KEY";

            options.AddSecurityDefinition(apiKeySecurity, new OpenApiSecurityScheme
            {
                Description = "API-KEY based authorization. Enter your API-KEY to access not public API",
                Scheme = "ApiKeyScheme",
                Name = apiKeySecurity,
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
            });

            var scheme = new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = apiKeySecurity
                },

                In = ParameterLocation.Header
            };

            var requirement = new OpenApiSecurityRequirement
            {
                { scheme, Array.Empty<string>() }
            };

            options.AddSecurityRequirement(requirement);
        });

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
