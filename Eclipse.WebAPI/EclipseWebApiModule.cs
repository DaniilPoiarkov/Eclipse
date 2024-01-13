using Eclipse.DataAccess;
using Eclipse.DataAccess.EclipseCosmosDb;
using Eclipse.WebAPI.Filters;
using Eclipse.WebAPI.Middlewares;

using Microsoft.OpenApi.Models;

using Swashbuckle.AspNetCore.SwaggerGen;

namespace Eclipse.WebAPI;

/// <summary>
/// Takes responsibility for WebAPI
/// </summary>
public static class EclipseWebApiModule
{
    public static IServiceCollection AddWebApiModule(this IServiceCollection services)
    {
        services.AddControllers()
            .AddNewtonsoftJson();

        services
            .AddEndpointsApiExplorer();

        services
            .AddScoped<ApiKeyAuthorizeAttribute>()
            .AddScoped<TelegramBotApiSecretTokenAuthorizeAttribute>();

        services.AddSwaggerGen(ConfigureSwagger);

        services
            .AddExceptionHandler<ExceptionHandlerMiddleware>()
            .AddProblemDetails();

        return services;
    }

    private static void ConfigureSwagger(SwaggerGenOptions options)
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "Eclipse",
            Description = "Open API to test an app. Some features might require special access via API-KEY",
            Version = "v1",
        });

        var apiKeySecurity = "X-API-KEY";

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
    }

    // TODO: Move to DataAccess module after adding ability to reference WebApplication here
    public static async Task InitializeDataAccessModule(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var serviceProvider = scope.ServiceProvider;

        var logger = serviceProvider.GetRequiredService<Serilog.ILogger>();
        var context = serviceProvider.GetRequiredService<EclipseCosmosDbContext>();

        logger.Information("Initializing {module} module", nameof(EclipseDataAccessModule));

        logger.Information("\tInitializing database");
        await context.InitializeAsync();

        logger.Information("{module} module initialized successfully", nameof(EclipseDataAccessModule));
    }
}
