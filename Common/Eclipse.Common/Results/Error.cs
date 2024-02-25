namespace Eclipse.Common.Results;

public sealed record Error
{
    public string Code { get; }

    public string Description { get; }

    public ErrorType Type { get; }

    private Error(string code, string description, ErrorType type)
    {
        Code = code;
        Description = description;
        Type = type;
    }

    public static readonly Error None = new(string.Empty, string.Empty, ErrorType.Failure);

    public static implicit operator Result(Error error) => Result.Failure(error);

    public static Error NotFound(string code, string description) => new(code, description, ErrorType.NotFound);
    public static Error Conflict(string code, string description) => new(code, description, ErrorType.Conflict);
    public static Error Validation(string code, string description) => new(code, description, ErrorType.Validation);
    public static Error Failure(string code, string description) => new(code, description, ErrorType.Failure);
}

public enum ErrorType
{
    Failure,
    Validation,
    NotFound,
    Conflict
}
