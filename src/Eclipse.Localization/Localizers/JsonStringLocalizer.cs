using Eclipse.Localization.Resources;

using Microsoft.Extensions.Localization;

namespace Eclipse.Localization.Localizers;

internal sealed class JsonStringLocalizer : IStringLocalizer
{
    private readonly LocalizationResource _resource;

    private readonly string? _location;

    public JsonStringLocalizer(LocalizationResource resource)
    {
        _resource = resource;
    }

    public JsonStringLocalizer(LocalizationResource resource, string? location) : this(resource)
    {
        _location = location;
    }

    public LocalizedString this[string name]
    {
        get
        {
            var value = GetStringSafely(name);
            return new LocalizedString(name, value ?? name, value is null, _location);
        }
    }

    public LocalizedString this[string name, params object[] arguments]
    {
        get
        {
            var template = this[name];

            if (template.ResourceNotFound)
            {
                return template;
            }

            var value = string.Format(template, arguments);
            return new LocalizedString(name, value);
        }
    }

    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
    {
        return _resource.Texts.Select(kv => new LocalizedString(kv.Key, kv.Value, false, _location));
    }

    private string? GetStringSafely(string name)
    {
        _resource.Texts.TryGetValue(name, out var value);
        return value;
    }
}
