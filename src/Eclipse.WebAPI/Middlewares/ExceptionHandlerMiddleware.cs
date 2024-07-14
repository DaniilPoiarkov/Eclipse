using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace Eclipse.WebAPI.Middlewares;

public sealed class ExceptionHandlerMiddleware : IExceptionHandler
{
    private readonly ILogger<ExceptionHandlerMiddleware> _logger;

    public ExceptionHandlerMiddleware(ILogger<ExceptionHandlerMiddleware> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
    {
        context.Response.StatusCode = exception switch
        {
            NotImplementedException => StatusCodes.Status501NotImplemented,
            _ => StatusCodes.Status500InternalServerError
        };

        var stringLocalizer = context.RequestServices.GetRequiredService<IStringLocalizer<ExceptionHandlerMiddleware>>();

        var problems = new ProblemDetails()
        {
            Status = context.Response.StatusCode,
            Title = stringLocalizer["InternalError_Title"],
            Detail = stringLocalizer["InternalError_Details"]
        };

        await context.Response.WriteAsJsonAsync(problems, cancellationToken);

        _logger.LogError(
            message: "Unhandled exception:\n{error}",
            args: stringLocalizer[exception.Message, exception.Data.Values]
        );

        return true;
    }
}
