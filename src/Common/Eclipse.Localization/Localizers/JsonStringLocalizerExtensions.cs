using Microsoft.Extensions.Localization;

using System.Diagnostics.CodeAnalysis;

namespace Eclipse.Localization.Localizers;

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

    public static bool TryConvertToLocalizableString(this IStringLocalizer stringLocalizer, string value, [NotNullWhen(true)] out string? key)
    {
        key = null;

        try
        {
            key = stringLocalizer.ToLocalizableString(value);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
