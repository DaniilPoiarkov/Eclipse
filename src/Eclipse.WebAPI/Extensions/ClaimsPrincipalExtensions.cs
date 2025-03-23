using Eclipse.Domain.Shared.Identity;

using System.Security.Claims;

namespace Eclipse.WebAPI.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal principal)
    {
        return principal.Parse<Guid>(ClaimTypes.NameIdentifier, default);
    }

    public static long GetChatId(this ClaimsPrincipal principal)
    {
        return principal.Parse<long>(CustomClaimTypes.ChatId, default);
    }

    private static T Parse<T>(this ClaimsPrincipal principal, string claimType, IFormatProvider? formatProvider)
        where T : IParsable<T>
    {
        var claim = principal.Claims.FirstOrDefault(claim => claim.Type == claimType);

        if (!T.TryParse(claim?.Value, formatProvider, out var value))
        {
            throw new InvalidOperationException($"Principal does not contain {claimType} claim.");
        }

        return value;
    }
}
