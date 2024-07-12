using Eclipse.Localization.Builder;
using Eclipse.Localization.Culture;
using Eclipse.Localization.Exceptions;
using Eclipse.Localization.Resources;

using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace Eclipse.Localization.Localizers;

internal sealed class JsonStringLocalizer : IStringLocalizer, ILocalizer
{
    private readonly IOptions<LocalizationBuilderV2> _options;

    private readonly IResourceProvider _resourceProvider;
    
    private readonly ICurrentCulture _currentCulture;

    private readonly string? _location;

    public string DefaultCulture => _options.Value.DefaultCulture;

    public JsonStringLocalizer(
        IOptions<LocalizationBuilderV2> options,
        IResourceProvider resourceProvider,
        ICurrentCulture currentCulture)
    {
        _options = options;
        _resourceProvider = resourceProvider;
        _currentCulture = currentCulture;
    }

    public JsonStringLocalizer(
        IOptions<LocalizationBuilderV2> options,
        IResourceProvider resourceProvider,
        ICurrentCulture currentCulture,
        string? location)
        : this(options, resourceProvider, currentCulture)
    {
        _location = location;
    }

    public LocalizedString this[string key, string? culture = null]
    {
        get
        {
            using var _ = _currentCulture.UsingCulture(culture ?? DefaultCulture);
            return this[key];
        }
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
        var culture = _currentCulture.Culture ?? _options.Value.DefaultCulture;

        var resource = _resourceProvider.Get(culture);

        return resource.Texts.Select(pair => new LocalizedString(pair.Key, pair.Value, false, _location));
    }

    public string FormatLocalizedException(LocalizedException exception, string? culture = null)
    {
        return culture is null
            ? FormatLocalizedExceptionInternal(exception)
            : FormatLocalizedExceptionInternal(exception, culture);
    }

    private string FormatLocalizedExceptionInternal(LocalizedException exception, string culture)
    {
        using var _ = _currentCulture.UsingCulture(culture);
        return FormatLocalizedExceptionInternal(exception);
    }

    private string FormatLocalizedExceptionInternal(LocalizedException exception)
    {
        return this[exception.Message, exception.Args];
    }

    public string ToLocalizableString(string value)
    {
        var resource = _resourceProvider.GetWithValue(value);
        return resource.Texts.FirstOrDefault(pair => pair.Value == value).Key;
    }

    private string? GetStringSafely(string key)
    {
        var culture = _currentCulture.Culture ?? _options.Value.DefaultCulture;

        var resource = _location is null
            ? _resourceProvider.Get(culture)
            : _resourceProvider.Get(culture, _location);

        resource.Texts.TryGetValue(key, out var value);

        return value;
    }
}
