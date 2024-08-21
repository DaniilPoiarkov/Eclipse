using Eclipse.Common.Background;
using Eclipse.Common.Session;
using Eclipse.Localization;
using Eclipse.WebAPI.Background;
using Eclipse.WebAPI.Configurations;
using Eclipse.WebAPI.Extensions;
using Eclipse.WebAPI.Filters.Authorization;
using Eclipse.WebAPI.Health;
using Eclipse.WebAPI.Middlewares;
using Eclipse.WebAPI.Session;

using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Eclipse.WebAPI;

/// <summary>
/// Takes responsibility for WebAPI
/// </summary>
public static class EclipseWebApiModule
{
    private static readonly TimeSpan _window = TimeSpan.FromSeconds(10);

    private static readonly int _segmentsPerWindow = 2;

    private static readonly int _permitLimit = 10;

    public static IServiceCollection AddWebApiModule(this IServiceCollection services)
    {
        var configuration = services.GetConfiguration();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(new JwtBearerOptionsConfiguration(configuration).Configure);

        services.AddAuthorization();

        services
            .AddControllers()
            .AddNewtonsoftJson();

        services
            .AddEndpointsApiExplorer();

        services
            .AddScoped<ApiKeyAuthorizeAttribute>()
            .AddScoped<TelegramBotApiSecretTokenAuthorizeAttribute>()
            .AddScoped<CurrentSessionResolverMiddleware>()
            .AddScoped<ErrorLocalizationMiddleware>()
            .AddScoped<CurrentSession>()
            .AddScoped<ICurrentSession>(sp => sp.GetRequiredService<CurrentSession>());

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
            .ConfigureOptions<ApplicationInsightsConfiguration>()
            .ConfigureOptions<AuthorizationConfiguration>();

        services.Scan(tss => tss.FromAssemblyOf<ImportEntitiesBackgroundJobArgs>()
            .AddClasses(c => c.AssignableTo(typeof(IBackgroundJob<>)))
            .AsSelf()
            .WithTransientLifetime());

        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            options
                .AddIpAddressSlidingWindow(_window, _segmentsPerWindow, _permitLimit)
                .AddIpAddressFiveMinutesWindow();
        });

        services.Configure<ApiKeyAuthorizationOptions>(
            configuration.GetSection("Authorization")
        );

        return services;
    }

    public static WebApplication InitializeWebApiModule(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI();

        if (app.Environment.IsDevelopment())
        {
            ///
        }

        app.UseExceptionHandler()
            .UseHttpsRedirection();

        app.UseEclipseHealthChecks();

        app.UseRateLimiter();

        app.UseAuthentication()
            .UseAuthorization();

        app.UseLocalization();

        app.UseMiddleware<CurrentSessionResolverMiddleware>()
            .UseMiddleware<ErrorLocalizationMiddleware>();

        app.MapControllers();

        return app;
    }
}
