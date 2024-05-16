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
    }
}
