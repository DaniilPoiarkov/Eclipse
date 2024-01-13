
using Eclipse.WebAPI.Filters.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Eclipse.WebAPI.Filters.Enpoints;

public sealed class ApiKeyAuthorizeEndpointFilter : IEndpointFilter
{
    private static readonly string _header = "X-API-KEY";

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var httpContext = context.HttpContext;

        var options = httpContext
            .RequestServices
            .GetRequiredService<IOptions<ApiKeyAuthorizationOptions>>();

        var response = httpContext.Response;

        if (!httpContext.Request.Headers.TryGetValue(_header, out var apiKey))
        {
            response.StatusCode = StatusCodes.Status401Unauthorized;
            await response.WriteAsJsonAsync(new
            {
                Error = "API-KEY is missing"
            });

            return default;
        }

        if (!options.Value.EclipseApiKey.Equals(apiKey))
        {
            response.StatusCode = StatusCodes.Status401Unauthorized;
            await response.WriteAsJsonAsync(new
            {
                Error = "API-KEY is invalid"
            });

            return default;
        }

        return await next(context);
    }
}
