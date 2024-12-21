using Eclipse.Localization.Builder;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

using System.Globalization;

namespace Eclipse.Localization.Culture;

internal sealed class CultureResolverMiddleware : IMiddleware
{
    private readonly IOptions<LocalizationBuilder> _options;

    public CultureResolverMiddleware(IOptions<LocalizationBuilder> options)
    {
        _options = options;
    }

    public Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        CultureInfo.CurrentUICulture = GetCulture(context);

        return next(context);
    }

    private CultureInfo GetCulture(HttpContext context)
    {
        if (!context.Request.Headers.TryGetValue("Content-Language", out var values))
        {
            return CultureInfo.GetCultureInfo(_options.Value.DefaultCulture);
        }

        var culture = values.FirstOrDefault();

        if (string.IsNullOrEmpty(culture))
        {
            return CultureInfo.GetCultureInfo(_options.Value.DefaultCulture);
        }

        return CultureInfo.GetCultureInfo(culture);
    }
}
