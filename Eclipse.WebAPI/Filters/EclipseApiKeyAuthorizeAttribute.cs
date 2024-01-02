using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;

namespace Eclipse.WebAPI.Filters;

public sealed class EclipseApiKeyAuthorizeAttribute : ApiKeyAuthorizeAttribute
{
    public EclipseApiKeyAuthorizeAttribute()
        : base("X-API-KEY") { }

    protected override string GetExpectedValue(AuthorizationFilterContext context)
    {
        var options = context.HttpContext
            .RequestServices
            .GetRequiredService<IOptions<ApiKeyAuthorizationOptions>>();

        return options.Value.EclipseApiKey;
    }
}
