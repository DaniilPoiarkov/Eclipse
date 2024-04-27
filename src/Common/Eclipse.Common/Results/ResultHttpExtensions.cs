using Eclipse.Common.Results;
using Eclipse.Common.Results.ErrorParsers;

using Microsoft.AspNetCore.Http;

namespace Microsoft.AspNetCore.Mvc;

public static class ResultHttpExtensions
{
    public static IActionResult ToProblemActionResult(this Result result)
    {
        var parser = result.ThrowOrGetErrorParser();

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

    public static IResult ToProblemResult(this Result result)
    {
        var parser = result.ThrowOrGetErrorParser();

        return Results.Problem(
            statusCode: parser.StatusCode,
            title: parser.Title,
            type: parser.Type,
            extensions: new Dictionary<string, object?>
            {
                ["errors"] = new[] { result.Error }
            });
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

    public static IActionResult ToActionResult(this Result result, Func<IActionResult> okResult)
    {
        return result.Match(okResult, result.ToProblemActionResult);
    }

    public static IResult ToResult(this Result result, Func<IResult> okResult)
    {
        return result.Match(okResult, result.ToProblemResult);
    }
}
