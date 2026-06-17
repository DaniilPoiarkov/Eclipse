using System.Text.Json.Serialization;

namespace Eclipse.MCP.Requests.TodoItems;

public sealed class TodoItemResponse
{
    [JsonPropertyName("id")]
    public Guid Id { get; init; }

    [JsonPropertyName("userId")]
    public Guid UserId { get; init; }

    [JsonPropertyName("text")]
    public string Text { get; init; } = string.Empty;

    [JsonPropertyName("isFinished")]
    public bool IsFinished { get; init; }

    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; init; }

    [JsonPropertyName("finishedAt")]
    public DateTime? FinishedAt { get; init; }
}
