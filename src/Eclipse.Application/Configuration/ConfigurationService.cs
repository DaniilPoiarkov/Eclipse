using Eclipse.Application.Contracts.Configuration;

using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace Eclipse.Application.Configuration;

internal sealed class ConfigurationService : IConfigurationService
{
    private readonly IOptions<CultureList> _cultureOptions;

    private readonly IStringLocalizer<ConfigurationService> _stringLocalizer;

    public ConfigurationService(
        IOptions<CultureList> cultureOptions,
        IStringLocalizer<ConfigurationService> stringLocalizer)
    {
        _cultureOptions = cultureOptions;
        _stringLocalizer = stringLocalizer;
    }

    public List<CultureInfo> GetCultures()
    {
        return [.. _cultureOptions.Value.Select(info =>
            new CultureInfo(_stringLocalizer[info.Culture], info.Code)
        )];
    }
}
