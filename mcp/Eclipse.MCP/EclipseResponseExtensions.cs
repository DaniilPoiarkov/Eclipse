using Eclipse.MCP.Core.Client;
using Eclipse.MCP.Core.Tools;

namespace Eclipse.MCP;

public static class EclipseResponseExtensions
{
    public static Error ToError<T>(this EclipseResponse<T> response) where T : class =>
        (int)response.StatusCode switch
        {
            400 or 422           => Error.Validation(response.Content),
            401 or 403           => Error.Permission(response.Content),
            429                  => Error.Transient(response.Content),
            >= 400 and < 500     => Error.Business(response.Content),
            >= 500 and < 600     => Error.Transient(response.Content),
            _                    => Error.Uncertain(response.Content),
        };

    public static ToolResponse<T> ToToolResponse<T>(this EclipseResponse<T> response) where T : class =>
        response.IsError
            ? ToolResponse<T>.Failure([response.ToError()])
            : ToolResponse<T>.Success(response.Data!);
}
