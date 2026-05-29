using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace Eclipse.MCP.Core.Client;

public sealed class EclipseResponse<T>
    where T : class
{
    [MemberNotNullWhen(false, nameof(IsError))]
    public T? Data { get; init; }
    public string Content { get; init; }

    public bool IsError { get; init; }

    public HttpStatusCode StatusCode { get; init; }

    private EclipseResponse(T? data, string content, bool isError, HttpStatusCode statusCode)
    {
        Data = data;
        Content = content;
        IsError = isError;
        StatusCode = statusCode;
    }

    public static EclipseResponse<T> Success(T data, string content, HttpStatusCode statusCode) => new(data, content, false, statusCode);
    public static EclipseResponse<T> Failure(string content, HttpStatusCode statusCode) => new(null, content, true, statusCode);
}
