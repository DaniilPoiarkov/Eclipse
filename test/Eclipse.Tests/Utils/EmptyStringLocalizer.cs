using Microsoft.Extensions.Localization;

namespace Eclipse.Tests.Utils;

public sealed class EmptyStringLocalizer<T> : IStringLocalizer<T>
{
    private readonly Dictionary<string, string> _values;

    public EmptyStringLocalizer(Dictionary<string, string> values)
    {
        _values = values;
    }

    public LocalizedString this[string name] => new(name, _values[name]);

    public LocalizedString this[string name, params object[] arguments] => new(name, string.Join(this[name], arguments));

    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
    {
        return _values.Select(x => new LocalizedString(x.Key, x.Value));
    }
}
