using Eclipse.Common.Background;
using Eclipse.WebAPI.Background;
using Eclipse.WebAPI.Configurations;
using Eclipse.WebAPI.Filters.Authorization;
using Eclipse.WebAPI.Middlewares;

namespace Eclipse.WebAPI;

/// <summary>
/// Takes responsibility for WebAPI
/// </summary>
public static class EclipseWebApiModule
{
    public static IServiceCollection AddWebApiModule(this IServiceCollection services)
    {
        var configuration = services.GetConfiguration();

        services
            .AddControllers()
            .AddNewtonsoftJson();

        services
            .AddEndpointsApiExplorer();

        services
            .AddScoped<ApiKeyAuthorizeAttribute>()
            .AddScoped<TelegramBotApiSecretTokenAuthorizeAttribute>();

        services.AddSwaggerGen();

        services
            .AddExceptionHandler<ExceptionHandlerMiddleware>()
            .AddProblemDetails();

        services.AddApiVersioning()
            .AddMvc()
            .AddApiExplorer();

        services.AddApplicationInsightsTelemetry();

        services
            .ConfigureOptions<ApiExplorerConfiguration>()
            .ConfigureOptions<ApiVersioningConfiguration>()
            .ConfigureOptions<SwaggerUIConfiguration>()
            .ConfigureOptions<SwaggerGenConfiguration>()
            .ConfigureOptions<ApplicationInsightsConfiguration>();

        services.Scan(tss => tss.FromAssemblyOf<ImportEntitiesBackgroundJobArgs>()
            .AddClasses(c => c.AssignableTo(typeof(IBackgroundJob<>)))
            .AsSelf()
            .WithTransientLifetime());

        return services;
    }
}
