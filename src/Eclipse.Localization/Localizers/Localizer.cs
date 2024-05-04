using Eclipse.Localization.Exceptions;

namespace Eclipse.Localization.Localizers;

internal sealed class Localizer : ILocalizer
{
    public string DefaultCulture { get; }

    private readonly List<LocalizationResource> _resources;

    private readonly LocalizationResource _default;

    public Localizer(List<LocalizationResource> resources, string defaultCulture)
    {
        _resources = resources;
        DefaultCulture = defaultCulture;
        _default = _resources.First(l => l.Culture == defaultCulture);
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
