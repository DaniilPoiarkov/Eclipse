using Microsoft.Extensions.Localization;

namespace Eclipse.Localization.Localizers;

internal sealed class TypedJsonStringLocalizer<TResourceType> : IStringLocalizer<TResourceType>
{
    private readonly IStringLocalizer _localizer;

    public TypedJsonStringLocalizer(IStringLocalizerFactory factory)
    {
        _localizer = factory.Create(typeof(TResourceType));
    }

    public LocalizedString this[string name] => _localizer[name];

    public LocalizedString this[string name, params object[] arguments] => _localizer[name, arguments];

    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
    {
        return _localizer.GetAllStrings(includeParentCultures);
    }
}
