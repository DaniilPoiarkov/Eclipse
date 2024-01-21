using Eclipse.Localization.StringResources;

namespace Eclipse.Localization.Builder;

public sealed class LocalizationOptions
{
    private readonly List<StringResourceDefinition> _resources = [];

    internal IReadOnlyList<StringResourceDefinition> Resources => _resources.AsReadOnly();

    public LocalizationOptions Add<TResource>(string path)
    {
        var resource = new StringResourceDefinition(typeof(TResource), path);
        _resources.Add(resource);

        return this;
    }
}
