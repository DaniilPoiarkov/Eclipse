using System.Text.Json.Serialization;

namespace Eclipse.MCP.Requests.Feedbacks;

public sealed class FeedbackResponse
{
    [JsonPropertyName("id")]
    public Guid Id { get; init; }

    [JsonPropertyName("userId")]
    public Guid UserId { get; init; }

    [JsonPropertyName("comment")]
    public string Comment { get; init; } = string.Empty;

    [JsonPropertyName("rate")]
    public string Rate { get; init; } = string.Empty;

    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; init; }
}
