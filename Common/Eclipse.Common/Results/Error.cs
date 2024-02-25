namespace Eclipse.Common.Results;

public sealed record Error
{
    /// <summary>
    /// Error code
    /// </summary>
    public string Code { get; }

    /// <summary>
    /// Localized description string
    /// </summary>
    public string Description { get; }

    /// <summary>
    /// Error types
    /// </summary>
    public ErrorType Type { get; }

    /// <summary>
    /// Localization args
    /// </summary>
    public object[] Args { get; }

    private Error(string code, string description, ErrorType type, object[] args)
    {
        Code = code;
        Description = description;
        Type = type;
        Args = args;
    }

    public static readonly Error None = new(string.Empty, string.Empty, ErrorType.Failure, []);

    public static implicit operator Result(Error? error) => Result.Failure(error ?? None);

    public static Error NotFound(string code, string description, params object[] args) => new(code, description, ErrorType.NotFound, args);
    public static Error Conflict(string code, string description, params object[] args) => new(code, description, ErrorType.Conflict, args);
    public static Error Validation(string code, string description, params object[] args) => new(code, description, ErrorType.Validation, args);
    public static Error Failure(string code, string description, params object[] args) => new(code, description, ErrorType.Failure, args);
}

public enum ErrorType
{
    Failure,
    Validation,
    NotFound,
    Conflict
}
