using Eclipse.Common.Results.ErrorParsers;

using Microsoft.AspNetCore.Mvc;

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

    public static IActionResult ToProblem(this Result result)
    {
        var parser = ThrowOrGetErrorParser(result);

        var problemDetails = new ProblemDetails
        {
            Status = parser.StatusCode,
            Title = parser.Title,
            Type = parser.Type,
            Extensions = new Dictionary<string, object?>
            {
                ["errors"] = new[] { result.Error }
            }
        };

        return new ObjectResult(problemDetails)
        {
            StatusCode = problemDetails.Status
        };
    }

    private static IErrorParser ThrowOrGetErrorParser(this Result result)
    {
        if (result.IsSuccess)
        {
            throw new InvalidOperationException($"Success Results cannot be converted to probled details.");
        }

        return GetErrorParser(result.Error!);
    }

    private static IErrorParser GetErrorParser(Error error)
    {
        return error.Type switch
        {
            ErrorType.Validation => new ValidationErrorParser(),
            ErrorType.NotFound => new NotFoundErrorParser(),
            ErrorType.Conflict => new ConflictErrorParser(),
            _ => new ServerErrorErrorParser(),
        };
    }
}
