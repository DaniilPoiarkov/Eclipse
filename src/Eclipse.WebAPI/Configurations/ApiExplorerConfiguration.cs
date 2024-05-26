using Asp.Versioning.ApiExplorer;

using Microsoft.Extensions.Options;

namespace Eclipse.WebAPI.Configurations;

public sealed class ApiExplorerConfiguration : IConfigureOptions<ApiExplorerOptions>
{
    public void Configure(ApiExplorerOptions options)
    {
        options.GroupNameFormat = "'v'VVV";
        options.SubstituteApiVersionInUrl = true;
    }
}
