namespace Eclipse.Application.Contracts.Authorization;

public sealed class LoginResult
{
    public string? AccessToken { get; set; }

    public long Expiration { get; set; }
}
