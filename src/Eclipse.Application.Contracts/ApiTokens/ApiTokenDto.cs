using Eclipse.Application.Contracts.Entities;
using Eclipse.Domain.Shared.ApiTokens;

namespace Eclipse.Application.Contracts.ApiTokens;

public sealed class ApiTokenDto : EntityDto
{
    public required string Name { get; init; }

    public string MaskedValue { get; init; } = string.Empty;

    public DateTime CreatedAt { get; init; }

    public DateTime ExpiresAt { get; init; }

    public IReadOnlyList<ApiTokenScope> Scopes { get; init; } = [];
}
