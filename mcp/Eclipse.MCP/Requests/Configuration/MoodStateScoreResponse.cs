using System.Text.Json.Serialization;

namespace Eclipse.MCP.Requests.Configuration;

public sealed class MoodStateScoreResponse
{
    [JsonPropertyName("state")]
    public string State { get; init; } = string.Empty;

    [JsonPropertyName("score")]
    public int Score { get; init; }
}
