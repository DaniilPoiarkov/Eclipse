using System.Text.Json.Serialization;

namespace Eclipse.MCP.Core.Client;

public class PaginatedResponse<T>
{
    [JsonPropertyName("items")]
    public IList<T> Items { get; init; } = [];

    [JsonPropertyName("pages")]
    public int Pages { get; init; }

    [JsonPropertyName("totalCount")]
    public int TotalCount { get; init; }

    [JsonPropertyName("count")]
    public int Count { get; init; }
}
