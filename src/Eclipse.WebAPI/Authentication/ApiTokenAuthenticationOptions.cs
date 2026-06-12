namespace Eclipse.WebAPI.Authentication;

public sealed class ApiTokenAuthenticationOptions
{
    public required string HeaderName { get; init; }

    public required string TokenPrefix { get; init; }
}
