using Eclipse.Domain.Shared.ApiTokens;

namespace Eclipse.Application.Contracts.ApiTokens;

public sealed record CreateApiTokenDto(
    string Name,
    IReadOnlyList<ApiTokenScope> Scopes
);
