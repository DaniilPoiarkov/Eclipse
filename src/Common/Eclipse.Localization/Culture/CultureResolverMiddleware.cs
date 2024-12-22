using Eclipse.Localization.Builder;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
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
        var resolvers = context.RequestServices.GetServices<ICultureResolver>();

        foreach (var resolver in resolvers)
        {
            if (resolver.TryGetCulture(context, out var culture))
            {
                return culture;
            }
        }

        return CultureInfo.GetCultureInfo(_options.Value.DefaultCulture);
    }
}
