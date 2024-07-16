using Eclipse.Localization.Culture;
using Eclipse.Localization.Localizers;

using Microsoft.Extensions.Localization;

namespace Eclipse.Localization.Extensions;

public static class JsonStringLocalizerExtensions
{
    public static void UseCurrentCulture<T>(this IStringLocalizer<T> stringLocalizer, ICurrentCulture currentCulture)
    {
        if (stringLocalizer is not TypedJsonStringLocalizer<T> jsonStringLocalizer)
        {
            throw new InvalidOperationException($"{nameof(ICurrentCulture)} usage is available only for {nameof(TypedJsonStringLocalizer<T>)} implementation.");
        }

        jsonStringLocalizer.UseCurrentCulture(currentCulture);
    }

    public static void UseCurrentCulture(this IStringLocalizer stringLocalizer, ICurrentCulture currentCulture)
    {
        if (stringLocalizer is not JsonStringLocalizer jsonStringLocalizer)
        {
            throw new InvalidOperationException($"{nameof(ICurrentCulture)} usage is available only for {nameof(JsonStringLocalizer)} implementation.");
        }

        jsonStringLocalizer.UseCurrentCulture(currentCulture);
    }

    public static string ToLocalizableString<T>(this IStringLocalizer<T> stringLocalizer, string value)
    {
        if (stringLocalizer is not TypedJsonStringLocalizer<T> jsonStringLocalizer)
        {
            throw new InvalidOperationException($"Converting to localizable string is available only for {nameof(TypedJsonStringLocalizer<T>)} implementation.");
        }

        return jsonStringLocalizer.ToLocalizableString(value);
    }

    public static string ToLocalizableString(this IStringLocalizer stringLocalizer, string value)
    {
        if (stringLocalizer is not JsonStringLocalizer jsonStringLocalizer)
        {
            throw new InvalidOperationException($"Converting to localizable string is available only for {nameof(JsonStringLocalizer)} implementation.");
        }

        return jsonStringLocalizer.ToLocalizableString(value);
    }
}
