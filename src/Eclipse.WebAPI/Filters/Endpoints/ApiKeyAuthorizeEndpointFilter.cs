using Eclipse.WebAPI.Filters.Authorization;

using Microsoft.Extensions.Options;

namespace Eclipse.WebAPI.Filters.Endpoints;

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
            await SendUnauthorizedResponse(response, "API-KEY is missing");
            return default;
        }

        if (!options.Value.EclipseApiKey.Equals(apiKey))
        {
            await SendUnauthorizedResponse(response, "API-KEY is invalid");
            return default;
        }

        return await next(context);
    }

    private static Task SendUnauthorizedResponse(HttpResponse response, string error)
    {
        response.StatusCode = StatusCodes.Status401Unauthorized;
        return response.WriteAsJsonAsync(new
        {
            Error = error
        });
    }
}
