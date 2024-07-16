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
}
