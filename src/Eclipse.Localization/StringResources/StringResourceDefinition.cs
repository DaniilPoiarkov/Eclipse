using Eclipse.Localization.Localizers;

namespace Eclipse.Localization.StringResources;

internal sealed class StringResourceDefinition
{
    public Type Type { get; }

    public string Path { get; }

    public StringResourceDefinition(Type type, string path)
    {
        Type = type;
        Path = path;
    }

    internal LocalizationResource GetCultureInfo(string path)
    {
        return new(); // TODO:...
    }
}
