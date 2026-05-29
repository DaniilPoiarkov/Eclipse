using System.Diagnostics.CodeAnalysis;

namespace Eclipse.MCP;

public sealed class ToolResponse<T>
    where T : class
{
    [MemberNotNullWhen(false, nameof(IsError))]
    public T? Data { get; }

    public bool IsError => Errors.Any();

    public IEnumerable<Error> Errors { get; init; } = [];

    private ToolResponse(T? data, IEnumerable<Error> errors)
    {
        Data = data;
        Errors = errors;
    }

    public static ToolResponse<T> Success(T data) => new(data, []);
    public static ToolResponse<T> Failure(IEnumerable<Error> errors) => new(null, errors);
}
