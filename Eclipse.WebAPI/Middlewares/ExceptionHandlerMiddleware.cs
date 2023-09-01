using System.Net;

namespace Eclipse.WebAPI.Middlewares;

public class ExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlerMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            var status = ex switch
            {
                _ => HttpStatusCode.InternalServerError
            };

            context.Response.StatusCode = (int)status;

            await context.Response.WriteAsJsonAsync(new { Error = ex.Message });
        }
    }
}
