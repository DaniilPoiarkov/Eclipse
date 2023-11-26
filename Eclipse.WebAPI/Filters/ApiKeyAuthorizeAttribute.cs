using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;

namespace Eclipse.WebAPI.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class ApiKeyAuthorizeAttribute : Attribute, IAuthorizationFilter
{
    private static readonly string HeaderName = "API-KEY";

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var options = context.HttpContext.RequestServices.GetRequiredService<IOptions<ApiKeyAuthorizationOptions>>();

        if (!context.HttpContext.Request.Headers.TryGetValue(HeaderName, out var apiKey))
        {
            context.Result = new UnauthorizedObjectResult("API-KEY is missing");
            return;
        }

        if (!options.Value.EclipseApiKey.Equals(apiKey))
        {
            context.Result = new UnauthorizedObjectResult("API-KEY is invalid");
            return;
        }
    }
}
