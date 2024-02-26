using Eclipse.Localization.Localizers;

using Microsoft.AspNetCore.Diagnostics;

using ILogger = Serilog.ILogger;

namespace Eclipse.WebAPI.Middlewares;

public sealed class ExceptionHandlerMiddleware : IExceptionHandler
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
            NotImplementedException => StatusCodes.Status501NotImplemented,
            _ => StatusCodes.Status500InternalServerError
        };

        context.Response.StatusCode = statusCode;

        await context.Response.WriteAsJsonAsync(
            new { Error = "Internal error." },
            cancellationToken: cancellationToken
        );

        if (statusCode == StatusCodes.Status500InternalServerError)
        {
            _logger.Error(
                _localizer[exception.Message, "en"],
                exception.Data.Values);
        }

        return true;
    }
}
