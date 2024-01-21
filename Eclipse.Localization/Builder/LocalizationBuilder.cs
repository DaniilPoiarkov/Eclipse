using Eclipse.Localization.Exceptions;
using Eclipse.Localization.Localizers;

using Newtonsoft.Json;

namespace Eclipse.Localization.Builder;

internal sealed class LocalizationBuilder : ILocalizationBuilder
{
    private readonly List<CultureInfo> _localizations = [];

    public string DefaultLocalization { get; set; } = "en";

    public ILocalizationBuilder AddJsonFile(string path)
    {
        var fullPath = Path.GetFullPath(path);

        if (!File.Exists(fullPath))
        {
            throw new LocalizationFileNotExistException(fullPath);
        }

        var json = File.ReadAllText(fullPath);

        var localizationResource = JsonConvert.DeserializeObject<CultureInfo>(json)
            ?? throw new UnableToParseLocalizationResourceException(path);

        if (localizationResource.Localization.Equals(string.Empty))
        {
            throw new UnableToParseLocalizationResourceException(path);
        }

        _localizations.Add(localizationResource);

        return this;
    }

    public ILocalizationBuilder AddJsonFiles(string path)
    {
        var fullPath = Path.GetFullPath(path);

        var cultureInfos = Directory.GetFiles(fullPath, "*.json")
            .Select(File.ReadAllText)
            .Select(JsonConvert.DeserializeObject<CultureInfo>)
            .Where(cultureInfo => cultureInfo is not null);

        foreach (var cultureInfo in cultureInfos)
        {
            var existing = _localizations.FirstOrDefault(l => l.Localization == cultureInfo!.Localization);

            if (existing is null)
            {
                _localizations.Add(cultureInfo!);
                continue;
            }

            existing.Texts = existing.Texts
                .Concat(cultureInfo!.Texts)
                .ToDictionary(kv => kv.Key, kv => kv.Value);
        }

        return this;
    }

    public ILocalizer Build() => new Localizer(_localizations, DefaultLocalization);
}
