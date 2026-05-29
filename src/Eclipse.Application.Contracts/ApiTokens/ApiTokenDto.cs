using Eclipse.Application.Contracts.Entities;

namespace Eclipse.Application.Contracts.ApiTokens;

public sealed class ApiTokenDto : EntityDto
{
    public required string Name { get; init; }

    public DateTime CreatedAt { get; init; }

    public DateTime ExpiresAt { get; init; }
}
