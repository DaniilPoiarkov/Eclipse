using Microsoft.AspNetCore.Http;

using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Eclipse.Localization.Culture;

public interface ICultureResolver
{
    bool TryGetCulture(HttpContext context, [NotNullWhen(true)] out CultureInfo? cultureInfo);
}
