using Eclipse.Application.Exceptions;
using Eclipse.Domain.Shared.Exceptions;

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
                DomainException => HttpStatusCode.Forbidden,
                ObjectNotFoundException => HttpStatusCode.NotFound,
                ApplicationException => HttpStatusCode.BadRequest,
                _ => HttpStatusCode.InternalServerError
            };

            context.Response.StatusCode = (int)status;

            await context.Response.WriteAsJsonAsync(new { Error = ex.Message });
        }
    }
}
