using System.Net;

namespace Eclipse.WebAPI.Middlewares;

public class ExceptionHandlerMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
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
