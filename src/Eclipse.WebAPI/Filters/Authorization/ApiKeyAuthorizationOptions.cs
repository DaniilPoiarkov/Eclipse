namespace Eclipse.WebAPI.Filters.Authorization;

public class ApiKeyAuthorizationOptions
{
    public string EclipseApiKey { get; set; } = Guid.NewGuid().ToString().Replace("-", string.Empty)[..6];
}
