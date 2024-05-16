using Asp.Versioning;

using Eclipse.WebAPI.Configurations;
using Eclipse.WebAPI.Filters.Authorization;
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
        var configuration = services.GetConfiguration();

        services
            .AddControllers()
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

        services.AddApiVersioning(options =>
        {
            var apiVersioningConfiguration = configuration.GetSection("ApiVersioningOptions");

            options.DefaultApiVersion = new ApiVersion(
                apiVersioningConfiguration.GetValue<double>("DefaultVersion")
            );

            options.ApiVersionReader = ApiVersionReader.Combine(
                new QueryStringApiVersionReader(),
                new HeaderApiVersionReader(apiVersioningConfiguration["Header"]!)
            );

            options.ReportApiVersions = apiVersioningConfiguration.GetValue<bool>("ReportApiVersions");
            options.AssumeDefaultVersionWhenUnspecified = apiVersioningConfiguration.GetValue<bool>("AssumeDefaultVersionWhenUnspecified");
        }).AddMvc()
        .AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
        });

        services
            .ConfigureOptions<SwaggerUIConfiguration>()
            .ConfigureOptions<SwaggerGenConfiguration>();

        return services;
    }

    private static void ConfigureSwagger(SwaggerGenOptions options)
    {
        var apiKeySecurity = "X-Api-Key";

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
}
