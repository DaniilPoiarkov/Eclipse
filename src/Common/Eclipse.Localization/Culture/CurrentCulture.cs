using Eclipse.Localization.Builder;

using Microsoft.Extensions.Options;

using System.Globalization;

namespace Eclipse.Localization.Culture;

internal sealed class CurrentCulture : ICurrentCulture
{
    public CultureInfo Culture { get; private set; }

    public CurrentCulture(IOptions<LocalizationBuilder> options)
    {
        Culture = new CultureInfo(options.Value.DefaultCulture);
    }

    internal void SetCulture(CultureInfo culture)
    {
        Culture = culture;
        CultureInfo.CurrentUICulture = culture;
    }

    public IDisposable UsingCulture(string culture)
    {
        return UsingCulture(new CultureInfo(culture));
    }

    public IDisposable UsingCulture(CultureInfo culture)
    {
        var scope = new CultureScope(Culture, this);

        Culture = culture;
        CultureInfo.CurrentUICulture = culture;

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
