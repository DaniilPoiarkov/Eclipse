namespace Eclipse.MCP.Core.Tools;

public sealed class Error
{
    public ToolErrorCode Type { get; init; }
    public string Message { get; init; }
    public bool IsRetryable { get; init; }

    private Error(ToolErrorCode type, string message, bool isRetryable)
    {
        Type = type;
        Message = message;
        IsRetryable = isRetryable;
    }

    public static Error Transient(string message) => new(ToolErrorCode.Transient, message, true);
    public static Error Business(string message) => new(ToolErrorCode.Business, message, false);
    public static Error Validation(string message) => new(ToolErrorCode.Validation, message, false);
    public static Error Permission(string message) => new(ToolErrorCode.Permission, message, false);
    public static Error Uncertain(string message) => new(ToolErrorCode.Uncertain, message, false);
}
