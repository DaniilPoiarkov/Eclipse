using System.Globalization;

namespace Eclipse.Localization.Culture;

public interface ICurrentCulture
{
    string Culture { get; }

    CultureInfo CurrectCulture { get; }

    /// <summary>
    /// Sets the culture in disposable scope.
    /// </summary>
    /// <param name="culture">The culture.</param>
    /// <returns></returns>
    IDisposable UsingCulture(string culture);

    IDisposable UsingCulture(CultureInfo culture);
}
