using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;

namespace Eclipse.WebAPI.Filters.Authorization;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class ApiKeyAuthorizeAttribute : ApiKeyAuthorizeBaseAttribute
{
    public ApiKeyAuthorizeAttribute()
        : base("X-Api-Key") { }

    protected override string GetExpectedValue(AuthorizationFilterContext context)
    {
        var options = context.HttpContext
            .RequestServices
            .GetRequiredService<IOptions<ApiKeyAuthorizationOptions>>();

        return options.Value.EclipseApiKey;
    }
}
