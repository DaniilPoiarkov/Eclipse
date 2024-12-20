using Eclipse.Localization.Culture;
using Eclipse.Localization.Resources;

using Microsoft.Extensions.Localization;

using System.Globalization;

namespace Eclipse.Localization.Localizers;

internal sealed class JsonStringLocalizer : IStringLocalizer, ILocalizedStringConverter
{
    private readonly IResourceProvider _resourceProvider;

    private readonly string? _location;

    public JsonStringLocalizer(IResourceProvider resourceProvider)
    {
        _resourceProvider = resourceProvider;
    }

    public JsonStringLocalizer(IResourceProvider resourceProvider, string? location)
        : this(resourceProvider)
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
            return new LocalizedString(name, value, false, _location);
        }
    }

    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
    {
        var resource = _resourceProvider.Get(CultureInfo.CurrentUICulture);

        return resource.Texts.Select(pair => new LocalizedString(pair.Key, pair.Value, false, _location));
    }

    public string ToLocalizableString(string value)
    {
        var resource = _resourceProvider.GetWithValue(value);
        return resource.Texts.FirstOrDefault(pair => pair.Value == value).Key;
    }

    private string? GetStringSafely(string key)
    {
        var resource = _location is null
            ? _resourceProvider.Get(CultureInfo.CurrentUICulture)
            : _resourceProvider.Get(CultureInfo.CurrentUICulture, _location);

        resource.Texts.TryGetValue(key, out var value);

        return value;
    }
}
