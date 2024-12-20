using System.Globalization;

namespace Eclipse.Localization.Culture;

public interface ICurrentCulture
{
    /// <summary>
    /// Usings the culture.
    /// </summary>
    /// <param name="culture">The culture.</param>
    /// <returns></returns>
    IDisposable UsingCulture(CultureInfo? culture);
}
