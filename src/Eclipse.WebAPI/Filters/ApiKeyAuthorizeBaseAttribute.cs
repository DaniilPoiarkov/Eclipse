using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Eclipse.WebAPI.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public abstract class ApiKeyAuthorizeBaseAttribute : Attribute, IAuthorizationFilter
{
    private readonly string _header;

    public ApiKeyAuthorizeBaseAttribute(string header)
    {
        _header = header;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var expectedValue = GetExpectedValue(context);

        if (!context.HttpContext.Request.Headers.TryGetValue(_header, out var apiKey))
        {
            context.Result = Unauthorized("API-KEY is missing");
            return;
        }

        if (!expectedValue.Equals(apiKey))
        {
            context.Result = Unauthorized("API-KEY is invalid");
            return;
        }
    }

    private static UnauthorizedObjectResult Unauthorized(string error) => new(new
    {
        Error = error
    });

    protected abstract string GetExpectedValue(AuthorizationFilterContext context);
}
