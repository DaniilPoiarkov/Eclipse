using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.Extensions.Options;

namespace Eclipse.WebAPI.Configurations;

public sealed class ApplicationInsightsConfiguration : IConfigureOptions<ApplicationInsightsServiceOptions>
{
    private readonly IConfiguration _configuration;

    public ApplicationInsightsConfiguration(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void Configure(ApplicationInsightsServiceOptions options)
    {
        options.ConnectionString = _configuration.GetConnectionString("ApplicationInsights");
    }
}
