namespace Eclipse.Core.Validation;

public class ValidationResult
{
    public bool IsSucceded { get; }

    public bool IsFailed => !IsSucceded;

    public string? ErrorMessage { get; }

    private ValidationResult(bool isSucceded, string? errorMessage)
    {
        IsSucceded = isSucceded;
        ErrorMessage = errorMessage;
    }

    public static ValidationResult Success() => new(true, null);

    public static ValidationResult Failure(string? errorMessage = null) => new(false, errorMessage);
}
