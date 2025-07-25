﻿using System.Globalization;

namespace Eclipse.Localization.Culture;

internal sealed class CurrentCulture : ICurrentCulture
{
    public IDisposable UsingCulture(CultureInfo? culture)
    {
        var scope = new CultureScope(CultureInfo.CurrentUICulture);

        if (culture is not null)
        {
            CultureInfo.CurrentUICulture = culture;
        }

        return scope;
    }

    private sealed class CultureScope : IDisposable
    {
        private readonly CultureInfo _fallbackCulture;

        public CultureScope(CultureInfo fallbackCulture)
        {
            _fallbackCulture = fallbackCulture;
        }

        public void Dispose()
        {
            CultureInfo.CurrentUICulture = _fallbackCulture;
        }
    }
}
