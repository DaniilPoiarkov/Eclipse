using Eclipse.Localization.Localizers;

namespace Eclipse.Localization.Builder;

internal sealed class LocalizationBuilerNew : ILocalizationBuilder
{
    private readonly List<string> _resources = [];

    public string DefaultLocalization { get; set; } = "en";

    public ILocalizationBuilder AddJsonFile(string path)
    {
        _resources.Add(path);
        return this;
    }

    public ILocalizationBuilder AddJsonFiles(string path)
    {
        _resources.Add(path);
        return this;
    }

    public ILocalizer Build()
    {
        return new LocalizerNew(_resources, DefaultLocalization);
    }
}
