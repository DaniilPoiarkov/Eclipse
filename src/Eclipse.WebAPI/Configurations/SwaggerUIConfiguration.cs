using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;

using Swashbuckle.AspNetCore.SwaggerUI;

namespace Eclipse.WebAPI.Configurations;

public sealed class SwaggerUIConfiguration : IConfigureOptions<SwaggerUIOptions>
{
    private readonly IApiVersionDescriptionProvider _provider;

    public SwaggerUIConfiguration(IApiVersionDescriptionProvider provider)
    {
        _provider = provider;
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
    }
}
