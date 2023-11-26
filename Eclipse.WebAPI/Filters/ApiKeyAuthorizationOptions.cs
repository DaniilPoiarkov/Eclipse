namespace Eclipse.WebAPI.Filters;

public class ApiKeyAuthorizationOptions
{
    public string EclipseApiKey { get; set; } = Guid.NewGuid().ToString().Replace("-", string.Empty)[..6];
}
