using System.Text.Json.Serialization;

namespace Eclipse.MCP.Requests.Suggestions;

public sealed class SuggestionUserResponse
{
    [JsonPropertyName("id")]
    public Guid Id { get; init; }

    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    [JsonPropertyName("surname")]
    public string Surname { get; init; } = string.Empty;

    [JsonPropertyName("userName")]
    public string UserName { get; init; } = string.Empty;

    [JsonPropertyName("chatId")]
    public long ChatId { get; init; }
}
