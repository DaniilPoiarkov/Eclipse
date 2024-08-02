namespace Eclipse.Application.Contracts.Authorization;

[Serializable]
public sealed class LoginResult
{
    public string? AccessToken { get; set; }

    public long Expiration { get; set; }
}
