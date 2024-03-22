using Eclipse.Localization.Exceptions;

namespace Eclipse.Localization.Localizers;

internal sealed class Localizer : ILocalizer
{
    private readonly List<LocalizationResource> _resources;

    private readonly LocalizationResource _default;

    public Localizer(List<LocalizationResource> resources, string @default)
    {
        _resources = resources;
        _default = _resources.First(l => l.Culture == @default);
    }

    public string this[string key, string? culture = null]
    {
        get
        {
            var localizer = _resources.FirstOrDefault(l => l.Culture == culture)
                ?? _default;

            return localizer.Texts.TryGetValue(key, out var localization)
                ? localization
                : key;
        }
    }

    public string FormatLocalizedException(LocalizedException exception, string? culture = null)
    {
        var message = this[exception.Message, culture];
        return string.Format(message, exception.Args.Select(a => this[a, culture]).ToArray());
    }

    public string ToLocalizableString(string value)
    {
        var localization = _resources.FirstOrDefault(l => l.Texts.ContainsValue(value))
            ?? throw new LocalizationNotFoundException(value, nameof(value));

        return localization.Texts.First(t => t.Value.Equals(value)).Key;
    }
}
