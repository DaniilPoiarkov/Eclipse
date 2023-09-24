using Eclipse.Localization.Localizers;

using Newtonsoft.Json;

namespace Eclipse.Localization.Builder;

internal class LocalizationBuilder : ILocalizationBuilder
{
    private readonly List<CultureInfo> _localizations = new();

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

    public ILocalizer Build() => new Localizer(_localizations, DefaultLocalization);
}
