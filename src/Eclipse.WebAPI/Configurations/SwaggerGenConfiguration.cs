using Asp.Versioning.ApiExplorer;

using Eclipse.WebAPI.Options;
using Eclipse.WebAPI.Swagger;

using Microsoft.Extensions.Options;
using Microsoft.OpenApi;

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
        options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
        {
            Description = "Azure Entra ID OAuth2 Authorization for administration purposes.",
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
        });

        options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("oauth2", document)] = []
        });
    }

    private static void ConfigureAuthorizationSecurityDefinition(SwaggerGenOptions options)
    {
        var apiKeySecurity = "Bearer";

        options.AddSecurityDefinition(apiKeySecurity, new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Description = "JWT Bearer token based authorization. Enter your token to access authorized API",
            Scheme = apiKeySecurity,
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey,
        });

        options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference(apiKeySecurity, document)] = []
        });
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
