using Eclipse.Localization.Localizers;

using Newtonsoft.Json;

namespace Eclipse.Localization.StringResources;

public sealed class StringResourceDefinition
{
    public Type Type { get; }

    public string Path { get; }

    public StringResourceDefinition(Type type, string path)
    {
        Type = type;
        Path = path;
    }

    internal LocalizationResource? GetCultureInfo(string culture)
    {
        var cultureInfo = Directory.GetFiles(System.IO.Path.GetFullPath(Path), "*.json")
            .Select(File.ReadAllText)
            .Where(json => json.StartsWith($"{{\"localization\": \"{culture}\"", StringComparison.CurrentCultureIgnoreCase))
            .Select(JsonConvert.DeserializeObject<LocalizationResource>)
            .Where(cultureInfo => cultureInfo is not null)
            .GroupBy(cultureInfo => cultureInfo!.Culture)
            .Select(grouping => new LocalizationResource
            {
                Culture = grouping.Key,
                Texts = grouping.SelectMany(cultureInfo => cultureInfo!.Texts).ToDictionary()
            })
            .SingleOrDefault();

        return cultureInfo;
    }   
}
