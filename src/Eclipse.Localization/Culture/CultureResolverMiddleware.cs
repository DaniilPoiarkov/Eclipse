using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;

namespace Eclipse.Localization.Culture;

internal sealed class CultureResolverMiddleware : IMiddleware
{
    private readonly CurrentCulture _currentCulture;

    private readonly IStringLocalizerFactory _localizationFactory;

    public CultureResolverMiddleware(CurrentCulture currentCulture, IStringLocalizerFactory localizationFactory)
    {
        _currentCulture = currentCulture;
        _localizationFactory = localizationFactory;
    }

    public Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (context.Request.Headers.TryGetValue("Content-Language", out var culture))
        {
            _currentCulture.SetCulture(culture!);
        }

        if (_localizationFactory is JsonStringLocalizerFactory jsonStringLocalizerFactory)
        {
            jsonStringLocalizerFactory.SetCurrentCulture(_currentCulture);
        }

        return next(context);
    }
}
