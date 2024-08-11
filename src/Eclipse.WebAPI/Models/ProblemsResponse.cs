using Eclipse.Common.Results;

using System.Text.Json.Serialization;

namespace Eclipse.WebAPI.Models;

public sealed class ProblemsResponse
{
    public string? Type { get; set; }
    public string? Title { get; set; }
    public string? Detail { get; set; }
    public string? Instance { get; set; }
    public int? Status { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Error[]? Errors { get; set; } = [];
}
