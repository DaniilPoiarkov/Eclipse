using Azure.Monitor.OpenTelemetry.AspNetCore;

using Microsoft.Extensions.Options;

namespace Eclipse.WebAPI.Configurations;

public sealed class AzureMonitorOptionsConfiguration : IConfigureOptions<AzureMonitorOptions>
{
    private readonly IConfiguration _configuration;

    public AzureMonitorOptionsConfiguration(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void Configure(AzureMonitorOptions options)
    {
        options.ConnectionString = _configuration.GetConnectionString("ApplicationInsights");
    }
}
