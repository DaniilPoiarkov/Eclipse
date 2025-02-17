namespace Eclipse.WebAPI.Options;

public sealed class AzureOAuthOptions
{
    public required string Instance { get; init; }
    
    public required string TenantId { get; init; }
    
    public required string ClientId { get; init; }

    public required AzureOAuthUrls Urls { get; init; }

    public List<AzureOAuthScope> Scopes { get; init; } = [];
}
