using Eclipse.Localization.Exceptions;

using Newtonsoft.Json;

namespace Eclipse.Localization.Localizers;

internal sealed class LocalizerNew : ILocalizer
{
    private readonly List<string> _resourcePaths;

    private readonly string _default;

    private readonly Lazy<List<LocalizationResource>> _resources;

    private List<LocalizationResource> Localizations => _resources.Value;

    public LocalizerNew(List<string> resourcePaths, string @default)
    {
        _resourcePaths = resourcePaths;
        _default = @default;
        _resources = new(GetCultureInfoList);
    }

    public string this[string key, string? culture = null]
    {
        get
        {
            var localizer = Localizations.FirstOrDefault(l => l.Culture == culture)
                ?? Localizations.First(l => l.Culture == _default);

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
        var localization = Localizations.FirstOrDefault(l => l.Texts.ContainsValue(value))
            ?? throw new LocalizationNotFoundException(value, nameof(value));

        return localization.Texts.First(t => t.Value.Equals(value)).Key;
    }

    private List<LocalizationResource> GetCultureInfoList()
    {
        var localizations = _resourcePaths.Select(Path.GetFullPath)
            .SelectMany(path => Directory.GetFiles(path, "*.json"))
            .Select(File.ReadAllText)
            .Select(JsonConvert.DeserializeObject<LocalizationResource>)
            .Where(cultureInfo => cultureInfo is not null)
            .GroupBy(cultureInfo => cultureInfo!.Culture)
            .Select(grouping => new LocalizationResource()
            {
                Culture = grouping.Key,
                Texts = grouping.SelectMany(cultureInfo => cultureInfo!.Texts).ToDictionary()
            })
            .ToList();

        return localizations;
    }
}
