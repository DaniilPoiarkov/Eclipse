using Asp.Versioning;

using Microsoft.Extensions.Options;

namespace Eclipse.WebAPI.Configurations;

public sealed class ApiVersioningConfiguration : IConfigureOptions<ApiVersioningOptions>
{
    private readonly IConfiguration _configuration;

    public ApiVersioningConfiguration(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void Configure(ApiVersioningOptions options)
    {
        var apiVersioningConfiguration = _configuration.GetSection("ApiVersioningOptions");

        options.DefaultApiVersion = new ApiVersion(
            apiVersioningConfiguration.GetValue<double>("DefaultVersion")
        );

        options.ApiVersionReader = ApiVersionReader.Combine(
            new UrlSegmentApiVersionReader(),
            new HeaderApiVersionReader(apiVersioningConfiguration["Header"]!)
        );

        options.ReportApiVersions = apiVersioningConfiguration.GetValue<bool>("ReportApiVersions");
        options.AssumeDefaultVersionWhenUnspecified = apiVersioningConfiguration.GetValue<bool>("AssumeDefaultVersionWhenUnspecified");
    }
}
