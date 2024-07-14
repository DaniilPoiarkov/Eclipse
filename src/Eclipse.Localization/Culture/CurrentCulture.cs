
using Eclipse.Localization.Builder;

using Microsoft.Extensions.Options;

namespace Eclipse.Localization.Culture;

internal sealed class CurrentCulture : ICurrentCulture
{
    public string Culture { get; private set; }

    public CurrentCulture(IOptions<LocalizationBuilder> options)
    {
        Culture = options.Value.DefaultCulture;
    }

    internal void SetCulture(string culture)
    {
        Culture = culture;
    }

    public IDisposable UsingCulture(string culture)
    {
        var scope = new CultureScope(Culture, this);

        Culture = culture;

        return scope;
    }

    private class CultureScope : IDisposable
    {
        private readonly string _fallbackCulture;

        private readonly CurrentCulture _currentCulture;

        public CultureScope(string fallbackCulture, CurrentCulture currentCulture)
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
