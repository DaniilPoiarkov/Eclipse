using Microsoft.Extensions.Localization;

namespace Eclipse.Localization.Localizers;

internal sealed class JsonStringLocalizer : IStringLocalizer
{
    private readonly LocalizationResource _resource;

    public JsonStringLocalizer(LocalizationResource locale)
    {
        _resource = locale;
    }

    public LocalizedString this[string name] => new(name, _resource.Texts[name]);

    public LocalizedString this[string name, params object[] arguments]
    {
        get
        {
            var template = this[name];
            var value = string.Format(template, arguments);
            return new LocalizedString(name, value);
        }
    }

    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
    {
        return _resource.Texts.Select(kv =>  new LocalizedString(kv.Key, kv.Value));
    }
}
