using System.Diagnostics.CodeAnalysis;

namespace Eclipse.Common.Results;

public class Result
{
    public bool IsSuccess { get; }

    [MemberNotNullWhen(false, nameof(IsSuccess))]
    public Error? Error { get; }

    protected Result(bool isSuccess, Error? error)
    {
        if (!AreArgumentsValid(isSuccess, error))
        {
            throw new InvalidOperationException($"Result cannot be \'{(isSuccess ? "Success" : "Failure")}\' with error {error}");
        }

        IsSuccess = isSuccess;
        Error = error;

        static bool AreArgumentsValid(bool isSuccess, Error? error)
        {
            return isSuccess && error is null
                || !isSuccess
                && error is not null
                && error != Error.None;
        }
    }

    public static Result Success() => new(true, null);
    public static Result Failure(Error error) => new(false, error);
}

public sealed class Result<TValue> : Result
{
    private readonly TValue? _value;
    public TValue Value { get
        {
            if (!IsSuccess || _value is null)
            {
                throw new InvalidOperationException($"Cannot access {nameof(Value)} when Result<{typeof(TValue).Name}> is failure.");
            }

            return _value;
        } }

    private Result(bool isSuccess, TValue? value) : base(isSuccess, null)
    {
        if (!isSuccess || value is null)
        {
            throw new InvalidOperationException(
                $"Result<{typeof(TValue).Name}> cannot be \'{(isSuccess ? "success" : "failure")}\' when providing {value?.GetType().AssemblyQualifiedName} value");
        }

        _value = value;
    }

    private Result(bool isSuccess, Error error)
        : base(isSuccess, error) { }

    public static Result<TValue> Success(TValue value) => new(true, value);
    public static new Result<TValue> Failure(Error error) => new(false, error);


    public static implicit operator Result<TValue>(TValue value) => Result<TValue>.Success(value);

    public static implicit operator Result<TValue>(Error error) => Result<TValue>.Failure(error);
}
