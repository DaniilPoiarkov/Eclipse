using Eclipse.Application.Exceptions;
using Eclipse.Domain.Exceptions;
using Eclipse.Localization.Localizers;

using Microsoft.AspNetCore.Diagnostics;

namespace Eclipse.WebAPI.Middlewares;

public class ExceptionHandlerMiddleware : IExceptionHandler
{
    private readonly ILocalizer _localizer;

    public ExceptionHandlerMiddleware(ILocalizer localizer)
    {
        _localizer = localizer;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
    {
        context.Response.StatusCode = exception switch
        {
            DomainException => StatusCodes.Status403Forbidden,
            EntityNotFoundException => StatusCodes.Status404NotFound,
            EclipseValidationException => StatusCodes.Status400BadRequest,
            NotImplementedException => StatusCodes.Status501NotImplemented,
            _ => StatusCodes.Status500InternalServerError
        };

        var template = _localizer[exception.Message];

        var error = string.Format(template, exception.Data.Values);

        await context.Response.WriteAsJsonAsync(
            new { Error = error },
            cancellationToken: cancellationToken);
        
        return true;
    }
}
