using Eclipse.Application.Exceptions;
using Eclipse.Domain.Shared.Exceptions;

using Microsoft.AspNetCore.Diagnostics;

using System.Net;

namespace Eclipse.WebAPI.Middlewares;

public class ExceptionHandlerMiddleware : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
    {
        var status = exception switch
        {
            DomainException => HttpStatusCode.Forbidden,
            ObjectNotFoundException => HttpStatusCode.NotFound,
            ApplicationException => HttpStatusCode.BadRequest,
            _ => HttpStatusCode.InternalServerError
        };

        context.Response.StatusCode = (int)status;

        await context.Response.WriteAsJsonAsync(new { Error = exception.Message }, cancellationToken: cancellationToken);
        
        return true;
    }
}
