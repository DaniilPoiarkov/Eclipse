using Asp.Versioning.ApiExplorer;

using Eclipse.WebAPI.Options;

using Microsoft.Extensions.Options;

using Swashbuckle.AspNetCore.SwaggerUI;

namespace Eclipse.WebAPI.Configurations;

public sealed class SwaggerUIConfiguration : IConfigureOptions<SwaggerUIOptions>
{
    private readonly IApiVersionDescriptionProvider _provider;

    private readonly IOptions<AzureOAuthOptions> _options;

    public SwaggerUIConfiguration(IApiVersionDescriptionProvider provider, IOptions<AzureOAuthOptions> options)
    {
        _provider = provider;
        _options = options;
    }

    public void Configure(SwaggerUIOptions options)
    {
        foreach (var description in _provider.ApiVersionDescriptions)
        {
            options.SwaggerEndpoint(
                $"/swagger/{description.GroupName}/swagger.json",
                $"Eclipse API {description.GroupName.ToUpperInvariant()}. {(description.IsDeprecated ? "[Deprecated]" : "")}"
            );
        }

        options.OAuthAppName("Eclipse");
        options.OAuthClientId(_options.Value.ClientId);
        options.OAuthUsePkce();
    }
}
