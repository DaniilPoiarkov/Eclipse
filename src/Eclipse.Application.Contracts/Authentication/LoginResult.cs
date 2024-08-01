namespace Eclipse.Application.Contracts.Authentication;

public sealed class LoginResult
{
    public string? AccessToken { get; set; }

    public long Expiration { get; set; }
}
