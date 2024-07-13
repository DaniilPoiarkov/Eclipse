using Eclipse.Application.Contracts.Localizations;
using Eclipse.Localization;
using Eclipse.Localization.Culture;
using Eclipse.Localization.Exceptions;

namespace Eclipse.Application.Localizations;

internal sealed class EclipseLocalizer : IEclipseLocalizer
{
    private readonly ILocalizer _localizer;

    private readonly ICurrentCulture _currentCulture;

    private string Culture { get; set; }

    public EclipseLocalizer(ILocalizer localizer, ICurrentCulture currentCulture)
    {
        _localizer = localizer;
        Culture = localizer.DefaultCulture;
        _currentCulture = currentCulture;
    }

    public string this[string key] => _localizer[key, _currentCulture.Culture];

    public string ToLocalizableString(string value) =>
        _localizer.ToLocalizableString(value);

    public string FormatLocalizedException(LocalizedException exception, string? culture = null) =>
        _localizer.FormatLocalizedException(exception, culture ?? Culture);
}
