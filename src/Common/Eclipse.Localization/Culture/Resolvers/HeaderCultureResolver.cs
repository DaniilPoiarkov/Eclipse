using Microsoft.AspNetCore.Http;

using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Eclipse.Localization.Culture.Resolvers;

public sealed class HeaderCultureResolver : ICultureResolver
{
    public bool TryGetCulture(HttpContext context, [NotNullWhen(true)] out CultureInfo? cultureInfo)
    {
        cultureInfo = null;

        if (!context.Request.Headers.TryGetValue("Content-Language", out var values))
        {
            return false;
        }

        var culture = values.FirstOrDefault();

        if (string.IsNullOrEmpty(culture))
        {
            return false;
        }

        cultureInfo = CultureInfo.GetCultureInfo(culture);

        return true;
    }
}
