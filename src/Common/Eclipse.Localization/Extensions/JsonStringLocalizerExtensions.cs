using Eclipse.Localization.Localizers;

using Microsoft.Extensions.Localization;

namespace Eclipse.Localization.Extensions;

public static class JsonStringLocalizerExtensions
{
    public static string ToLocalizableString(this IStringLocalizer stringLocalizer, string value)
    {
        if (stringLocalizer is not ILocalizedStringConverter jsonStringLocalizer)
        {
            throw new InvalidOperationException($"Converting to localizable string is available only for {nameof(ILocalizedStringConverter)} implementation.");
        }

        return jsonStringLocalizer.ToLocalizableString(value);
    }
}
