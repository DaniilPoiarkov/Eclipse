using System.Text.Json.Serialization;

namespace Eclipse.MCP.Requests.Health;

public sealed class HealthResponse
{
    [JsonPropertyName("status")]
    public string Status { get; init; } = string.Empty;
}
