using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Eclipse.Application.ApiTokens.Expiration;

internal sealed class ApiTokenExpirationNotificationOptionsConfiguration : IConfigureOptions<ApiTokenExpirationNotificationOptions>
{
    private readonly IConfiguration _configuration;

    public ApiTokenExpirationNotificationOptionsConfiguration(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void Configure(ApiTokenExpirationNotificationOptions options)
    {
        _configuration.GetSection("ApiTokenExpiration").Bind(options);
    }
}
