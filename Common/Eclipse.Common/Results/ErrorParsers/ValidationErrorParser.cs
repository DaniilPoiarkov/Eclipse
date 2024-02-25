using Microsoft.AspNetCore.Http;

namespace Eclipse.Common.Results.ErrorParsers;

internal sealed class ValidationErrorParser : IErrorParser
{
    public int StatusCode => StatusCodes.Status400BadRequest;

    public string Title => "Bad request.";

    public string Type => "swagger/index.html";
}
