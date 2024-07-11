namespace Eclipse.Localization.Builder;

internal sealed class LocalizationBuilderV2 : ILocalizationBuilder
{
    private readonly List<string> _resources = [];

    internal IReadOnlyList<string> Resources => _resources.AsReadOnly();

    public string DefaultCulture { get; set; } = "en";

    public ILocalizationBuilder AddJsonFile(string path)
    {
        return AddJsonFiles(path);
    }

    public ILocalizationBuilder AddJsonFiles(string path)
    {
        _resources.Add(path);
        return this;
    }
}
