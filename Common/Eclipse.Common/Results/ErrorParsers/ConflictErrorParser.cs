using Microsoft.AspNetCore.Http;

namespace Eclipse.Common.Results.ErrorParsers;

internal sealed class ConflictErrorParser : IErrorParser
{
    public int StatusCode => StatusCodes.Status409Conflict;

    public string Title => "Conflict.";

    public string Type => "swagger/index.html";
}
