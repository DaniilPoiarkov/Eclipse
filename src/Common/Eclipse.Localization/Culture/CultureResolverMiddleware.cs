using Microsoft.AspNetCore.Http;

using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Eclipse.Localization.Culture;

internal sealed class CultureResolverMiddleware : IMiddleware
{
    private readonly CurrentCulture _currentCulture;

    public CultureResolverMiddleware(CurrentCulture currentCulture)
    {
        _currentCulture = currentCulture;
    }

    public Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (TryGetCulture(context, out var culture))
        {
            CultureInfo.CurrentUICulture = new CultureInfo(culture);
            _currentCulture.SetCulture(CultureInfo.CurrentUICulture);
        }

        return next(context);
    }

    private static bool TryGetCulture(HttpContext context, [NotNullWhen(true)] out string? culture)
    {
        culture = null;

        if (!context.Request.Headers.TryGetValue("Content-Language", out var values))
        {
            return false;
        }

        culture = values.FirstOrDefault();

        return !string.IsNullOrEmpty(culture);
    }
}
