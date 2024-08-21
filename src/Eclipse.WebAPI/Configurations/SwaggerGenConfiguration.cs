using Asp.Versioning.ApiExplorer;

using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;

using Swashbuckle.AspNetCore.SwaggerGen;

namespace Eclipse.WebAPI.Configurations;

public sealed class SwaggerGenConfiguration : IConfigureOptions<SwaggerGenOptions>
{
    private readonly IApiVersionDescriptionProvider _provider;

    public SwaggerGenConfiguration(IApiVersionDescriptionProvider provider)
    {
        _provider = provider;
    }

    public void Configure(SwaggerGenOptions options)
    {
        foreach (var description in _provider.ApiVersionDescriptions)
        {
            var openApiInfo = new OpenApiInfo
            {
                Title = $"Eclipse.Api v{description.ApiVersion}",
                Version = description.ApiVersion.ToString(),
                Description = "Open API to test an app. Some features might require special access via API-KEY"
            };

            options.SwaggerDoc(description.GroupName, openApiInfo);
        }

        ConfigureApiKeySecurityDefinition(options);
        ConfigureAuthorizationSecurityDefinition(options);
    }

    private static void ConfigureApiKeySecurityDefinition(SwaggerGenOptions options)
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

    private static void ConfigureAuthorizationSecurityDefinition(SwaggerGenOptions options)
    {
        var apiKeySecurity = "Bearer";

        options.AddSecurityDefinition(apiKeySecurity, new OpenApiSecurityScheme
        {
            Description = "JWT Bearer token based authorization. Enter your token to access authorized API",
            Scheme = apiKeySecurity,
            Name = "Authorization",
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
