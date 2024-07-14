using Eclipse.Localization;

using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Eclipse.WebAPI.Middlewares;

public sealed class ExceptionHandlerMiddleware : IExceptionHandler
{
    private readonly ILocalizer _localizer;

    private readonly ILogger<ExceptionHandlerMiddleware> _logger;

    public ExceptionHandlerMiddleware(ILocalizer localizer, ILogger<ExceptionHandlerMiddleware> logger)
    {
        _localizer = localizer;
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
    {
        context.Response.StatusCode = exception switch
        {
            NotImplementedException => StatusCodes.Status501NotImplemented,
            _ => StatusCodes.Status500InternalServerError
        };

        var problems = new ProblemDetails()
        {
            Status = context.Response.StatusCode,
            Title = "Internal error.",
            Detail = "Internal error occured.",
        };

        await context.Response.WriteAsJsonAsync(problems, cancellationToken);

        _logger.LogError(
            message: "Unhandled exception:\n{error}",
            args: string.Format(_localizer[exception.Message, "en"], exception.Data.Values)
        );

        return true;
    }
}
