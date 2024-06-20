using Eclipse.Application.Contracts.Url;

using Microsoft.Extensions.Configuration;

namespace Eclipse.Application.Url;

internal sealed class AppUrlProvider : IAppUrlProvider
{
    private readonly Lazy<string> _appUrl;
    public string AppUrl => _appUrl.Value;


    private readonly IConfiguration _configuration;

    public AppUrlProvider(IConfiguration configuration)
    {
        _configuration = configuration;
        _appUrl = new(() => GetAppSection().GetValue<string>("SelfUrl") ?? throw new InvalidOperationException("App.SelfUrl configuration is not provided."));
    }

    private IConfigurationSection GetAppSection()
    {
        return _configuration.GetRequiredSection("App");
    }
}
