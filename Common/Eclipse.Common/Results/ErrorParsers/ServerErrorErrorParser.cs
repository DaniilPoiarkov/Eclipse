using Microsoft.AspNetCore.Http;

namespace Eclipse.Common.Results.ErrorParsers;

internal sealed class ServerErrorErrorParser : IErrorParser
{
    public int StatusCode => StatusCodes.Status500InternalServerError;

    public string Title => "Server error.";

    public string Type => "swagger/index.html";
}
