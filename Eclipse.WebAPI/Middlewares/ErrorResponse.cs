using Eclipse.Infrastructure.Exceptions;

namespace Eclipse.WebAPI.Middlewares;

[Serializable]
public sealed class ErrorResponse
{
    public string Message { get; }

    public string? Code { get; }

    private ErrorResponse(string message, string? code)
    {
        Message = message;
        Code = code;
    }

    public static ErrorResponse Create(string message, Exception exception)
    {
        var code = exception is EclipseException eclipseException
                ? eclipseException.Code
                : null;

        return new ErrorResponse(message, code);
    }
}
