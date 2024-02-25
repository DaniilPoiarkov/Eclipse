using Eclipse.Common.Exceptions;
using Eclipse.Domain.Exceptions;
using Eclipse.Localization.Localizers;

using Microsoft.AspNetCore.Diagnostics;

using ILogger = Serilog.ILogger;

namespace Eclipse.WebAPI.Middlewares;

public class ExceptionHandlerMiddleware : IExceptionHandler
{
    private readonly ILocalizer _localizer;

    private readonly ILogger _logger;

    public ExceptionHandlerMiddleware(ILocalizer localizer, ILogger logger)
    {
        _localizer = localizer;
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
    {
        var statusCode = exception switch
        {
            EntityNotFoundException => StatusCodes.Status404NotFound,
            EclipseValidationException => StatusCodes.Status400BadRequest,
            NotImplementedException => StatusCodes.Status501NotImplemented,
            _ => StatusCodes.Status500InternalServerError
        };

        var template = _localizer[exception.Message, "en"];

        var error = statusCode == StatusCodes.Status500InternalServerError
            ? "Internal error."
            : string.Format(
                template,
                [..exception.Data.Values]
            );

        context.Response.StatusCode = statusCode;

        await context.Response.WriteAsJsonAsync(
            new { Error = ErrorResponse.Create(error, exception) },
            cancellationToken: cancellationToken
        );

        if (statusCode == StatusCodes.Status500InternalServerError)
        {
            _logger.Error(template, exception.Data.Values);
        }

        return true;
    }
}
