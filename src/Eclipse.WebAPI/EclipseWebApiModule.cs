using Eclipse.Application.Contracts.Configuration;
using Eclipse.Common.Background;
using Eclipse.Localization;
using Eclipse.WebAPI.Background;
using Eclipse.WebAPI.Configurations;
using Eclipse.WebAPI.Extensions;
using Eclipse.WebAPI.Filters;
using Eclipse.WebAPI.Health;
using Eclipse.WebAPI.Middlewares;
using Eclipse.WebAPI.Options;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;

using System.Text.Json.Serialization;

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
            .AddJwtBearer(EclipseDefaults.AuthenticationScheme, new JwtBearerOptionsConfiguration(configuration).Configure)
            .AddMicrosoftIdentityWebApi(configuration);

        services.AddAuthorization();

        services
            .AddControllers()
            .AddJsonOptions(jsonOptions =>
            {
                jsonOptions.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

        services.ConfigureTelegramBotMvc();

        services
            .AddEndpointsApiExplorer();

        services
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
            .ConfigureOptions<ApplicationInsightsConfiguration>()
            .ConfigureOptions<AuthorizationConfiguration>();

        services.Scan(tss => tss.FromAssemblyOf<ImportEntitiesBackgroundJobArgs>()
            .AddClasses(c => c.AssignableTo(typeof(IBackgroundJob<>)), publicOnly: false)
            .AsSelf()
            .WithTransientLifetime());

        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            options
                .AddIpAddressSlidingWindow(_window, _segmentsPerWindow, _permitLimit)
                .AddIpAddressFiveMinutesWindow();
        });

        services.Configure<AzureOAuthOptions>(
            configuration.GetSection("AzureAd")
        );

        services.Configure<CultureList>(
            configuration.GetSection(nameof(CultureList))
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

        app.MapControllers();

        return app;
    }
}
