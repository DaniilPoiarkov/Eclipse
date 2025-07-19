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
    public static T Match<T>(this Result result, Func<T> ok, Func<Error, T> error)
    {
        ArgumentNullException.ThrowIfNull(ok, nameof(ok));
        ArgumentNullException.ThrowIfNull(error, nameof(error));

        return result.IsSuccess ? ok() : error(result.Error);
    }

    public static TResult Match<TResult, TValue>(this Result<TValue> result, Func<TValue, TResult> ok, Func<Error, TResult> error)
    {
        ArgumentNullException.ThrowIfNull(ok, nameof(ok));
        ArgumentNullException.ThrowIfNull(error, nameof(error));

        return result.IsSuccess ? ok(result.Value) : error(result.Error);
    }

    /// <summary>
    /// Bind current result to another result.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="K"></typeparam>
    /// <param name="result">The result.</param>
    /// <param name="bind">The bind.</param>
    /// <returns></returns>
    public static async Task<Result<T>> BindAsync<T, K>(this Task<Result<K>> result, Func<K, T> bind)
    {
        var res = await result;

        return res.IsSuccess
            ? bind(res.Value)
            : res.Error;
    }

    /// <summary>
    /// Execute action and return current result.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="result">The result.</param>
    /// <param name="action">The action.</param>
    /// <returns></returns>
    public static async Task<Result<T>> TapAsync<T>(this Result<T> result, Func<T, Task> action)
    {
        if (result.IsSuccess)
        {
            await action(result.Value);
        }

        return result;
    }

    /// <summary>
    /// Execute action and return current result.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="result">The result.</param>
    /// <param name="action">The action.</param>
    /// <returns></returns>
    public static async Task<Result<T>> TapAsync<T>(this Task<Result<T>> result, Func<T, Task> action)
    {
        var res = await result;

        if (res.IsSuccess)
        {
            await action(res.Value);
        }

        return res;
    }
}
