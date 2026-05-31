using Eclipse.Domain.Shared.ApiTokens;
using Eclipse.Domain.Shared.Entities;

namespace Eclipse.Domain.ApiTokens;

public sealed class ApiToken : AggregateRoot, IHasCreatedAt
{
    public Guid UserId { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string TokenHash { get; private set; } = string.Empty;

    public DateTime CreatedAt { get; private set; }

    public DateTime ExpiresAt { get; private set; }

    private ApiToken() { }

    private ApiToken(Guid id, Guid userId, string name, string tokenHash, DateTime createdAt, DateTime expiresAt)
        : base(id)
    {
        UserId = userId;
        Name = name;
        TokenHash = tokenHash;
        CreatedAt = createdAt;
        ExpiresAt = expiresAt;
    }

    public static (ApiToken Token, string Plaintext) Create(Guid userId, string name, DateTime createdAt)
    {
        var plaintext = ApiTokenGenerator.Generate();

        var token = new ApiToken(
            Guid.CreateVersion7(),
            userId,
            name,
            ApiTokenGenerator.Hash(plaintext),
            createdAt,
            createdAt.Add(ApiTokensConsts.DefaultExpiration)
        );

        return (token, plaintext);
    }

    public bool IsExpired(DateTime utcNow) => utcNow >= ExpiresAt;
}
