namespace Eclipse.WebAPI.Options;

public sealed class AzureOAuthUrls
{
    public required Uri Authorization { get; init; }

    public required Uri Token { get; init; }

    public required Uri Refresh { get; init; }
}
