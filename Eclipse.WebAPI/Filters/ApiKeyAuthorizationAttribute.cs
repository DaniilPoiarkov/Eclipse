using Eclipse.WebAPI.Services.TelegramServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;

namespace Eclipse.WebAPI.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class ApiKeyAuthorizationAttribute : Attribute, IAuthorizationFilter
{
    private static readonly string HeaderName = "API-KEY";

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var options = context.HttpContext.RequestServices.GetRequiredService<IOptions<TelegramOptions>>();

        if (!context.HttpContext.Request.Headers.TryGetValue(HeaderName, out var apiKey))
        {
            context.Result = new UnauthorizedObjectResult("API-KEY is missing");
            return;
        }

        if (!options.Value.EclipseToken.Equals(apiKey))
        {
            context.Result = new UnauthorizedObjectResult("API-KEY is invalid");
            return;
        }
    }
}
