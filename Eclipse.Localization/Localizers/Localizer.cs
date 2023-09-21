using Eclipse.Localization.Exceptions;

namespace Eclipse.Localization.Localizers;

internal class Localizer : ILocalizer
{
    private readonly List<CultureInfo> _localizations;

    private readonly CultureInfo _default;

    public Localizer(List<CultureInfo> localizations, string @default)
    {
        _localizations = localizations;
        _default = _localizations.First(l => l.Localization == @default);
    }

    public string this[string key, string? culture = null]
    {
        get
        {
            var localizer = _localizations.FirstOrDefault(l => l.Localization == culture)
                ?? _default;

            return localizer.Texts.TryGetValue(key, out var localization)
                ? localization
                : key;
        }
    }

    public string FormatLocalizedException(LocalizedException exception, string? culture = null)
    {
        var message = this[exception.Message, culture];
        return string.Format(message, exception.Args);
    }
}
