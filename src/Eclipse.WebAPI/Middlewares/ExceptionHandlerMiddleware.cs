using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace Eclipse.WebAPI.Middlewares;

public sealed class ExceptionHandlerMiddleware : IExceptionHandler
{
    private readonly ILogger<ExceptionHandlerMiddleware> _logger;

    private readonly IStringLocalizer<ExceptionHandlerMiddleware> _stringLocalizer;

    public ExceptionHandlerMiddleware(ILogger<ExceptionHandlerMiddleware> logger, IStringLocalizer<ExceptionHandlerMiddleware> stringLocalizer)
    {
        _logger = logger;
        _stringLocalizer = stringLocalizer;
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
            Title = _stringLocalizer["InternalError_Title"],
            Detail = _stringLocalizer["InternalError_Title"]
        };

        await context.Response.WriteAsJsonAsync(problems, cancellationToken);

        _logger.LogError(
            message: "Unhandled exception:\n{error}",
            args: _stringLocalizer[exception.Message, exception.Data.Values]
        );

        return true;
    }
}
