using System.Text.Json.Serialization;

namespace Eclipse.MCP.Requests.Users;

public sealed class UserResponse
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

    [JsonPropertyName("culture")]
    public string Culture { get; init; } = string.Empty;

    [JsonPropertyName("notificationsEnabled")]
    public bool NotificationsEnabled { get; init; }

    [JsonPropertyName("isEnabled")]
    public bool IsEnabled { get; init; }

    [JsonPropertyName("gmt")]
    public string Gmt { get; init; } = string.Empty;

    [JsonPropertyName("createdAt")]
    public DateTime? CreatedAt { get; init; }
}
