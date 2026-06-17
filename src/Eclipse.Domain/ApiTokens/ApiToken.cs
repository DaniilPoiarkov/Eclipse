using Eclipse.Domain.Shared.ApiTokens;
using Eclipse.Domain.Shared.Entities;

namespace Eclipse.Domain.ApiTokens;

public sealed class ApiToken : AggregateRoot, IHasCreatedAt
{
    public Guid UserId { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string TokenHash { get; private set; } = string.Empty;

    public string MaskedValue { get; private set; } = string.Empty;

    public DateTime CreatedAt { get; private set; }

    public DateTime ExpiresAt { get; private set; }

    public IReadOnlyList<ApiTokenScope> Scopes { get; private set; } = [];

    private ApiToken() { }

    private ApiToken(Guid id, Guid userId, string name, string tokenHash, string maskedValue, DateTime createdAt, DateTime expiresAt, IReadOnlyList<ApiTokenScope> scopes)
        : base(id)
    {
        UserId = userId;
        Name = name;
        TokenHash = tokenHash;
        MaskedValue = maskedValue;
        CreatedAt = createdAt;
        ExpiresAt = expiresAt;
        Scopes = scopes;
    }

    public static (ApiToken Token, string Plaintext) Create(Guid userId, string name, DateTime createdAt, IEnumerable<ApiTokenScope> scopes)
    {
        var plaintext = ApiTokenGenerator.Generate();

        var token = new ApiToken(
            Guid.CreateVersion7(),
            userId,
            name,
            ApiTokenGenerator.Hash(plaintext),
            ApiTokenGenerator.Mask(plaintext),
            createdAt,
            createdAt.Add(ApiTokensConsts.DefaultExpiration),
            [.. scopes]
        );

        return (token, plaintext);
    }

    public bool IsExpired(DateTime utcNow) => utcNow >= ExpiresAt;
}
