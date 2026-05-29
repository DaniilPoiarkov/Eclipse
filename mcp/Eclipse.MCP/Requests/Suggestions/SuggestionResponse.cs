using System.Text.Json.Serialization;

namespace Eclipse.MCP.Requests.Suggestions;

public sealed class SuggestionResponse
{
    [JsonPropertyName("id")]
    public Guid Id { get; init; }

    [JsonPropertyName("text")]
    public string Text { get; init; } = string.Empty;

    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; init; }

    [JsonPropertyName("user")]
    public SuggestionUserResponse? User { get; init; }
}
