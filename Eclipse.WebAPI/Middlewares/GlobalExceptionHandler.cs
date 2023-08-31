using System.Net;

namespace Eclipse.WebAPI.Middlewares;

public class GlobalExceptionHandler
{
    private readonly RequestDelegate _next;

    public GlobalExceptionHandler(RequestDelegate next)
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
