namespace Eclipse.Common.Results;

public static class ResultExtensions
{
    /// <summary>
    /// Converts <a cref="result"></a> to <typeparamref name="T"/> based on <a cref="Result.IsSuccess"></a> status
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="result"></param>
    /// <param name="ok"></param>
    /// <param name="error"></param>
    /// <returns>
    /// </returns>
    public static T Match<T>(this Result result, Func<T> ok, Func<T> error)
    {
        ArgumentNullException.ThrowIfNull(ok, nameof(ok));
        ArgumentNullException.ThrowIfNull(error, nameof(error));

        return result.IsSuccess ? ok() : error();
    }

    public static TResult Match<TResult, TValue>(this Result<TValue> result, Func<TValue, TResult> ok, Func<Error, TResult> error)
    {
        ArgumentNullException.ThrowIfNull(ok, nameof(ok));
        ArgumentNullException.ThrowIfNull(error, nameof(error));

        return result.IsSuccess ? ok(result.Value) : error(result.Error);
    }
}
