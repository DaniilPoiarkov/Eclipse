﻿using Eclipse.Localization.Culture;
using Eclipse.Localization.Exceptions;
using Eclipse.Localization.Resources;

using Microsoft.Extensions.Localization;

namespace Eclipse.Localization.Localizers;

internal sealed class JsonStringLocalizer : IStringLocalizer
{
    private readonly IResourceProvider _resourceProvider;

    private ICurrentCulture CurrentCulture { get; set; }

    private readonly string? _location;

    public JsonStringLocalizer(
        IResourceProvider resourceProvider,
        ICurrentCulture currentCulture)
    {
        _resourceProvider = resourceProvider;
        CurrentCulture = currentCulture;
    }

    public JsonStringLocalizer(
        IResourceProvider resourceProvider,
        ICurrentCulture currentCulture,
        string? location)
        : this(resourceProvider, currentCulture)
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
        var culture = CurrentCulture.Culture;

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
        using var _ = CurrentCulture.UsingCulture(culture);
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
        var resource = _location is null
            ? _resourceProvider.Get(CurrentCulture.Culture)
            : _resourceProvider.Get(CurrentCulture.Culture, _location);

        resource.Texts.TryGetValue(key, out var value);

        return value;
    }

    public void UseCurrentCulture(ICurrentCulture currentCulture)
    {
        CurrentCulture = currentCulture;
    }
}
