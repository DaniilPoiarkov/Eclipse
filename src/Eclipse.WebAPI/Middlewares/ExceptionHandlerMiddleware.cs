using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace Eclipse.WebAPI.Middlewares;

public sealed class ExceptionHandlerMiddleware : IExceptionHandler
{
    private readonly ILogger<ExceptionHandlerMiddleware> _logger;

    private readonly IProblemDetailsService _problemDetailsService;

    public ExceptionHandlerMiddleware(
        ILogger<ExceptionHandlerMiddleware> logger,
        IProblemDetailsService problemDetailsService)
    {
        _logger = logger;
        _problemDetailsService = problemDetailsService;
    }

    public ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
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
            Instance = context.Request.Path,
            Title = stringLocalizer["InternalError_Title"],
            Detail = stringLocalizer["InternalError_Details"],
        };

        _logger.LogError(exception, "Unhandled exception: {Error}", exception.Message);

        return _problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            Exception = exception,
            HttpContext = context,
            ProblemDetails = problems
        });
    }
}
