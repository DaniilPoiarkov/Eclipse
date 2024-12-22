using System.Globalization;

namespace Eclipse.Localization.Resources;

internal interface IResourceProvider
{
    /// <summary>
    /// Gets the resource with specified culture using provided in options json files.
    /// </summary>
    /// <param name="culture">The culture.</param>
    /// <returns></returns>
    LocalizationResource Get(CultureInfo culture);

    /// <summary>
    /// Gets the resource with specified culture using provided location.
    /// </summary>
    /// <param name="culture">The culture.</param>
    /// <param name="location">The location.</param>
    /// <returns></returns>
    LocalizationResource Get(CultureInfo culture, string location);

    /// <summary>
    /// Gets the resource which contains the specified value using provided in options json files.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns></returns>
    LocalizationResource GetWithValue(string value);
}
