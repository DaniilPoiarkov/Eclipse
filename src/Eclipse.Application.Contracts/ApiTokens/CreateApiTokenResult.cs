namespace Eclipse.Application.Contracts.ApiTokens;

public sealed record CreateApiTokenResult(
    ApiTokenDto Token,
    string Plaintext
);
