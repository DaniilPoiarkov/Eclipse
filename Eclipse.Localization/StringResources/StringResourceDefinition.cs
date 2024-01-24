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

    internal CultureInfo? GetCultureInfo(string culture)
    {
        var cultureInfo = Directory.GetFiles(System.IO.Path.GetFullPath(Path), "*.json")
            .Select(File.ReadAllText)
            .Where(json => json.StartsWith($"{{\"localization\": \"{culture}\"", StringComparison.CurrentCultureIgnoreCase))
            .Select(JsonConvert.DeserializeObject<CultureInfo>)
            .Where(cultureInfo => cultureInfo is not null)
            .GroupBy(cultureInfo => cultureInfo!.Localization)
            .Select(grouping => new CultureInfo
            {
                Localization = grouping.Key,
                Texts = grouping.SelectMany(cultureInfo => cultureInfo!.Texts).ToDictionary()
            })
            .SingleOrDefault();

        return cultureInfo;
    }   
}
