using Microsoft.AspNetCore.Http;

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
        if (context.Request.Headers.TryGetValue("Content-Language", out var culture))
        {
            _currentCulture.SetCulture(culture!);
        }

        return next(context);
    }
}
