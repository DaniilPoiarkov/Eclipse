using System.Globalization;

namespace Eclipse.Localization.Culture;

public static class CurrentCultureExtensions
{
    /// <summary>
    /// Sets the culture in disposable scope.
    /// </summary>
    /// <param name="culture">The culture.</param>
    /// <returns></returns>
    public static IDisposable UsingCulture(this ICurrentCulture currentCulture, string culture)
    {
        return currentCulture.UsingCulture(new CultureInfo(culture));
    }
}
