using Eclipse.WebAPI.Filters;
using Microsoft.OpenApi.Models;

namespace Eclipse.WebAPI.Helpers;

public static class DependencyInjection
{
    public static IServiceCollection AddCore(this IServiceCollection services)
    {
        services.Scan(tss =>
            tss.FromAssemblyOf<WeatherForecast>()
                .AddClasses()
                .AsMatchingInterface()
                .WithTransientLifetime());

        services.AddControllers();

        services
            .AddEndpointsApiExplorer();

        services.AddScoped<ApiKeyAuthorizeAttribute>();

        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Eclipse",
                Description = "Open API to test an app. Some features might require special access via API-KEY",
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
}
