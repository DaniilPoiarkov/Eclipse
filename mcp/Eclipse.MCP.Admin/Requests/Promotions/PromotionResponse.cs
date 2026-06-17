using System.Text.Json.Serialization;

namespace Eclipse.MCP.Admin.Requests.Promotions;

public sealed class PromotionResponse
{
    [JsonPropertyName("id")]
    public Guid Id { get; init; }

    [JsonPropertyName("title")]
    public string Title { get; init; } = string.Empty;

    [JsonPropertyName("fromChatId")]
    public long FromChatId { get; init; }

    [JsonPropertyName("messageId")]
    public int MessageId { get; init; }

    [JsonPropertyName("inlineButtonText")]
    public string InlineButtonText { get; init; } = string.Empty;

    [JsonPropertyName("inlineButtonLink")]
    public string InlineButtonLink { get; init; } = string.Empty;

    [JsonPropertyName("timesPublished")]
    public int TimesPublished { get; init; }

    [JsonPropertyName("status")]
    public string Status { get; init; } = string.Empty;

    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; init; }
}
