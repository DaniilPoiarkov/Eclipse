using System.Text.Json.Serialization;

namespace Eclipse.MCP.Requests.Reminders;

public sealed class ReminderResponse
{
    [JsonPropertyName("id")]
    public Guid Id { get; init; }

    [JsonPropertyName("userId")]
    public Guid UserId { get; init; }

    [JsonPropertyName("relatedItemId")]
    public Guid? RelatedItemId { get; init; }

    [JsonPropertyName("text")]
    public string Text { get; init; } = string.Empty;

    [JsonPropertyName("notifyAt")]
    public string NotifyAt { get; init; } = string.Empty;
}
