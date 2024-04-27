using Microsoft.AspNetCore.Http;

namespace Eclipse.Common.Results.ErrorParsers;

internal sealed class NotFoundErrorParser : IErrorParser
{
    public int StatusCode => StatusCodes.Status404NotFound;

    public string Title => "Not found.";

    public string Type => "swagger/index.html";
}
