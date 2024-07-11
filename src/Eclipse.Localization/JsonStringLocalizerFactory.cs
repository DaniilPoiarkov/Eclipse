using Eclipse.Localization.Builder;
using Eclipse.Localization.Culture;
using Eclipse.Localization.Exceptions;
using Eclipse.Localization.Localizers;
using Eclipse.Localization.Resources;

using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace Eclipse.Localization;

internal sealed class JsonStringLocalizerFactory : IStringLocalizerFactory, ILocalizer
{
    private readonly ICurrentCulture _currentCulture;

    private readonly IResourceProvider _resourceProvider;

    public string DefaultCulture { get; }

    public JsonStringLocalizerFactory(IOptions<LocalizationBuilderV2> options, ICurrentCulture currentCulture, IResourceProvider resourceProvider)
    {
        _currentCulture = currentCulture;
        _resourceProvider = resourceProvider;

        DefaultCulture = options.Value.DefaultCulture;
    }

    #region IStringLocalizerFactory

    public IStringLocalizer Create(Type resourceSource)
    {
        var culture = _currentCulture.Culture ?? DefaultCulture;

        var resource = _resourceProvider.Get(culture);

        return new JsonStringLocalizer(resource);
    }

    public IStringLocalizer Create(string baseName, string location)
    {
        var culture = _currentCulture.Culture ?? DefaultCulture;

        var resource = _resourceProvider.Get(culture, location);

        return new JsonStringLocalizer(resource, location);
    }

    #endregion

    #region ILocalizer

    public LocalizedString this[string key, string? culture = null]
    {
        get
        {
            using var _ = _currentCulture.UsingCulture(culture ?? DefaultCulture);
            var localizer = Create(GetType());
            return localizer[key];
        }
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
        var localizer = Create(GetType());
        return localizer[exception.Message, exception.Args];
    }

    public string ToLocalizableString(string value)
    {
        var resource = _resourceProvider.GetWithValue(value);

        var pair = resource.Texts.FirstOrDefault(p => p.Value == value);

        return pair.Key;
    }

    #endregion
}
