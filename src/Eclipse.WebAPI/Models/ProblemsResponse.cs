using Eclipse.Common.Results;

namespace Eclipse.WebAPI.Models;

public sealed class ProblemsResponse
{
    public string? Type { get; set; }
    public string? Title { get; set; }
    public int? Status { get; set; }
    public string? Detail { get; set; }
    public string? Instance { get; set; }

    public Error? Error { get; set; }
}
