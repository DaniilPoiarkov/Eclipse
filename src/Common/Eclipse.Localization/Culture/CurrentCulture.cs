using Eclipse.Localization.Builder;

using Microsoft.Extensions.Options;

using System.Globalization;

namespace Eclipse.Localization.Culture;

internal sealed class CurrentCulture : ICurrentCulture
{
    public string Culture { get; private set; }

    public CultureInfo CurrectCulture { get; private set; }

    public CurrentCulture(IOptions<LocalizationBuilder> options)
    {
        Culture = options.Value.DefaultCulture;
        CurrectCulture = new CultureInfo(Culture);
    }

    internal void SetCulture(CultureInfo culture)
    {
        CurrectCulture = culture;
    }

    public IDisposable UsingCulture(string culture)
    {
        return UsingCulture(new CultureInfo(culture));
    }

    public IDisposable UsingCulture(CultureInfo culture)
    {
        var scope = new CultureScope(CurrectCulture, this);

        CurrectCulture = culture;

        return scope;
    }

    private class CultureScope : IDisposable
    {
        private readonly CultureInfo _fallbackCulture;

        private readonly CurrentCulture _currentCulture;

        public CultureScope(CultureInfo fallbackCulture, CurrentCulture currentCulture)
        {
            _fallbackCulture = fallbackCulture;
            _currentCulture = currentCulture;
        }

        public void Dispose()
        {
            _currentCulture.SetCulture(_fallbackCulture);
        }
    }
}
