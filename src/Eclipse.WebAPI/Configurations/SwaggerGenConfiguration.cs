using Asp.Versioning.ApiExplorer;

using Eclipse.WebAPI.Options;
using Eclipse.WebAPI.Swagger;

using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;

using Swashbuckle.AspNetCore.SwaggerGen;

namespace Eclipse.WebAPI.Configurations;

public sealed class SwaggerGenConfiguration : IConfigureOptions<SwaggerGenOptions>
{
    private readonly IApiVersionDescriptionProvider _provider;

    private readonly IOptions<AzureOAuthOptions> _options;

    public SwaggerGenConfiguration(IApiVersionDescriptionProvider provider, IOptions<AzureOAuthOptions> options)
    {
        _provider = provider;
        _options = options;
    }

    public void Configure(SwaggerGenOptions options)
    {
        ConfigureApiVersioning(options);
        ConfigureAuthorizationSecurityDefinition(options);
        ConfigureAzureEntraIdAuthentication(options);

        options.AddOperationFilterInstance(new ContentLanguageHeaderFilter());
        options.AddOperationFilterInstance(new DescriptionFilter());
    }

    private void ConfigureAzureEntraIdAuthentication(SwaggerGenOptions options)
    {
        var scheme = new OpenApiSecurityScheme
        {
            Reference = new OpenApiReference
            {
                Type = ReferenceType.SecurityScheme,
                Id = "oauth2"
            },
            Scheme = "oauth2",
            Name = "oauth2",
            In = ParameterLocation.Header
        };

        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            { scheme , new List<string>() }
        });

        var definition = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.OAuth2,
            Flows = new OpenApiOAuthFlows
            {
                AuthorizationCode = new OpenApiOAuthFlow
                {
                    AuthorizationUrl = _options.Value.Urls.Authorization,
                    TokenUrl = _options.Value.Urls.Token,
                    RefreshUrl = _options.Value.Urls.Refresh,
                    Scopes = _options.Value.Scopes.ToDictionary(s => s.Name, s => s.Description)
                }
            }
        };

        options.AddSecurityDefinition("oauth2", definition);
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

    private void ConfigureApiVersioning(SwaggerGenOptions options)
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
    }
}
