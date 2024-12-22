namespace Eclipse.Pipelines;

public sealed class PipelinesOptions
{
    public required string Token { get; init; }
    public required long Chat { get; init; }
    public required string SecretToken { get; init; }
    public required string ActiveEndpoint { get; init; }
}
