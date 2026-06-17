using System.Text.Json.Serialization;

namespace Eclipse.MCP.Requests.UserStatistics;

public sealed class UserStatisticsResponse
{
    [JsonPropertyName("id")]
    public Guid Id { get; init; }

    [JsonPropertyName("userId")]
    public Guid UserId { get; init; }

    [JsonPropertyName("todoItemsFinished")]
    public int TodoItemsFinished { get; init; }

    [JsonPropertyName("remindersReceived")]
    public int RemindersReceived { get; init; }
}
