namespace Eclipse.Localization.Builder;

internal sealed class LocalizationBuilder : ILocalizationBuilder
{
    private readonly List<string> _resources = [];

    internal IReadOnlyList<string> Resources => _resources.AsReadOnly();

    public string DefaultCulture { get; set; } = "en";

    public ILocalizationBuilder AddJsonFiles(string path)
    {
        _resources.Add(path);
        return this;
    }
}
