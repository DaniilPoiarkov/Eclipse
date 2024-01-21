using Microsoft.Extensions.Localization;

namespace Eclipse.Localization.Localizers;

internal sealed class JsonStringLocalizer : IStringLocalizer
{
    private readonly CultureInfo _locale;

    public JsonStringLocalizer(CultureInfo locale)
    {
        _locale = locale;
    }

    public LocalizedString this[string name] => new(name, _locale.Texts[name]);

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
        return _locale.Texts.Select(kv =>  new LocalizedString(kv.Key, kv.Value));
    }
}
