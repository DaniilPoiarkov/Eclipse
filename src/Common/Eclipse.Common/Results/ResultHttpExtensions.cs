using Eclipse.Common.Results;
using Eclipse.Common.Results.ErrorParsers;

using Microsoft.Extensions.Localization;

#pragma warning disable IDE0130

namespace Microsoft.AspNetCore.Mvc;

public static class ResultHttpExtensions
{
    public static IActionResult ToProblems(this Result result, IStringLocalizer stringLocalizer)
    {
        ArgumentNullException.ThrowIfNull(stringLocalizer, nameof(stringLocalizer));

        var parser = GetErrorParserOrThrow(result);

        var problemDetails = new ProblemDetails
        {
            Status = parser.StatusCode,
            Title = parser.Title,
            Type = parser.Type,
            Detail = stringLocalizer[result.Error.Description, result.Error.Args]
        };

        return new ObjectResult(problemDetails)
        {
            StatusCode = problemDetails.Status
        };
    }

    private static IErrorParser GetErrorParserOrThrow(Result result)
    {
        if (result.IsSuccess)
        {
            throw new InvalidOperationException($"Success Results cannot be converted to probled details.");
        }

        return GetErrorParser(result.Error);
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
