using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Eclipse.Localization.Culture;

internal sealed class CultureResolverMiddleware : IMiddleware
{
    private readonly CurrentCulture _currentCulture;

    public CultureResolverMiddleware(IServiceProvider serviceProvider)
    {
        _currentCulture = serviceProvider.GetRequiredService<CurrentCulture>();// currentCulture;
    }

    public Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (context.Request.Headers.TryGetValue("Content-Language", out var culture))
        {
            _currentCulture.SetCulture(culture);
        }

        return next(context);
    }
}
