using Eclipse.Localization.Localizers;

using Microsoft.Extensions.Localization;

namespace Eclipse.Localization.Extensions;

public static class JsonStringLocalizerExtensions
{
    public static IDisposable UsingCulture(this IStringLocalizer stringLocalizer, string culture)
    {
        if (stringLocalizer is not ICanUseCulture canUseCulture)
        {
            throw new InvalidOperationException($"Using culture is available only for {nameof(ICanUseCulture)} implementation.");
        }

        return canUseCulture.UsingCulture(culture);
    }

    public static string ToLocalizableString(this IStringLocalizer stringLocalizer, string value)
    {
        if (stringLocalizer is not ILocalizedStringConverter jsonStringLocalizer)
        {
            throw new InvalidOperationException($"Converting to localizable string is available only for {nameof(ILocalizedStringConverter)} implementation.");
        }

        return jsonStringLocalizer.ToLocalizableString(value);
    }
}
