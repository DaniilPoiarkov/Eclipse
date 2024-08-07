﻿namespace Eclipse.Common.Results;

public class Result
{
    public bool IsSuccess { get; }

    private readonly Error? _error;

    public Error Error
    {
        get
        {
            if (IsSuccess || _error is null)
            {
                throw new InvalidOperationException($"Cannot access {nameof(Error)} {(IsSuccess ? "when Result is success." : "as it was not provided.")}.");
            }

            return _error;
        }
    }

    protected Result(bool isSuccess, Error? error)
    {
        if (!AreArgumentsValid(isSuccess, error))
        {
            throw new InvalidOperationException($"Result cannot be \'{(isSuccess ? "Success" : "Failure")}\' with error {error}");
        }

        IsSuccess = isSuccess;
        _error = error;

        static bool AreArgumentsValid(bool isSuccess, Error? error)
        {
            return isSuccess && error is null
                || !isSuccess && error is not null;
        }
    }

    public static Result Success() => new(true, null);
    public static Result Failure(Error error) => new(false, error);
}

public sealed class Result<TValue> : Result
{
    private readonly TValue? _value;
    public TValue Value
    {
        get
        {
            if (!IsSuccess || _value is null)
            {
                throw new InvalidOperationException($"Cannot access {nameof(Value)} when Result<{typeof(TValue).Name}> is failure.");
            }

            return _value;
        }
    }

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

    public static implicit operator Result<TValue>(Error? error) => Result<TValue>.Failure(error ?? Error.None);

    public static implicit operator TValue(Result<TValue> result) => result.Value;
}
