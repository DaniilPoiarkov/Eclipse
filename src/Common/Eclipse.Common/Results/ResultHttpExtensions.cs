using Eclipse.Common.Results;
using Eclipse.Common.Results.ErrorParsers;

using Microsoft.Extensions.Localization;

#pragma warning disable IDE0130

namespace Microsoft.AspNetCore.Mvc;

public static class ResultHttpExtensions
{
    public static IActionResult ToProblems(this Error error, IStringLocalizer stringLocalizer)
    {
        var parser = GetErrorParser(error);

        var problemDetails = new ProblemDetails
        {
            Status = parser.StatusCode,
            Title = parser.Title,
            Type = parser.Type,
            Detail = stringLocalizer[error.Description, error.Args]
        };

        return new ObjectResult(problemDetails)
        {
            StatusCode = problemDetails.Status
        };
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
