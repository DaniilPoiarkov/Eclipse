using System.Text.Json;
using System.Text.Json.Serialization;

namespace Eclipse.MCP.Admin.Requests.Health;

public sealed class HealthResponse
{
    [JsonPropertyName("status")]
    public string Status { get; init; } = string.Empty;

    [JsonPropertyName("totalDuration")]
    public string TotalDuration { get; init; } = string.Empty;

    [JsonPropertyName("entries")]
    public Dictionary<string, HealthEntry<JsonElement>> Entries { get; init; } = [];
}
