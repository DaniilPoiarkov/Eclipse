using Eclipse.Localization.StringResources;

namespace Eclipse.Localization.Builder;

public sealed class LocalizationOptions
{
    private readonly List<StringResourceDefinition> _resources = [];

    internal IReadOnlyList<StringResourceDefinition> Resources => _resources.AsReadOnly();

    public LocalizationOptions Add<TResource>(string path)
    {
        return Add(typeof(TResource), path);
    }

    public LocalizationOptions Add(Type type, string path)
    {
        var resource = new StringResourceDefinition(type, path);
        _resources.Add(resource);

        return this;
    }
}
