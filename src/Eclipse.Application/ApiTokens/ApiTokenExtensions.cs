using Eclipse.Application.Contracts.ApiTokens;
using Eclipse.Domain.ApiTokens;

namespace Eclipse.Application.ApiTokens;

internal static class ApiTokenExtensions
{
    public static ApiTokenDto ToDto(this ApiToken token) => new()
    {
        Id = token.Id,
        Name = token.Name,
        MaskedValue = token.MaskedValue,
        CreatedAt = token.CreatedAt,
        ExpiresAt = token.ExpiresAt,
        Scopes = token.Scopes
    };
}
