using Eclipse.Localization.Exceptions;
using Eclipse.Localization.Localizers;

using Newtonsoft.Json;

namespace Eclipse.Localization.Builder;

internal sealed class LocalizationBuilder : ILocalizationBuilder
{
    private readonly List<LocalizationResource> _resources = [];

    public string DefaultLocalization { get; set; } = "en";

    public ILocalizationBuilder AddJsonFile(string path)
    {
        var fullPath = Path.GetFullPath(path);

        if (!File.Exists(fullPath))
        {
            throw new LocalizationFileNotExistException(fullPath);
        }

        var json = File.ReadAllText(fullPath);

        var resource = JsonConvert.DeserializeObject<LocalizationResource>(json);

        if (resource is null || string.IsNullOrEmpty(resource.Culture))
        {
            throw new UnableToParseLocalizationResourceException(path);
        }

        _resources.Add(resource);

        return this;
    }

    public ILocalizationBuilder AddJsonFiles(string path)
    {
        var fullPath = Path.GetFullPath(path);

        var resources = Directory.GetFiles(fullPath, "*.json")
            .Select(File.ReadAllText)
            .Select(JsonConvert.DeserializeObject<LocalizationResource>)
            .Where(resource => resource is not null)
            .GroupBy(resource => resource!.Culture)
            .Select(group => new LocalizationResource
            {
                Culture = group.Key,
                Texts = group
                    .SelectMany(r => r!.Texts)
                    .ToDictionary()
            });

        foreach (var resource in resources)
        {
            var existing = _resources.FirstOrDefault(l => l.Culture == resource!.Culture);

            if (existing is null)
            {
                _resources.Add(resource);
                continue;
            }

            existing.Texts = existing.Texts
                .Concat(resource.Texts)
                .ToDictionary();
        }

        return this;
    }

    public ILocalizer Build() => new Localizer(_resources, DefaultLocalization);
}
