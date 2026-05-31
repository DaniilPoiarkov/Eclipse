using System.Text.Json.Serialization;

namespace Eclipse.MCP.Requests.Configuration;

public sealed class CultureResponse
{
    [JsonPropertyName("culture")]
    public string Culture { get; init; } = string.Empty;

    [JsonPropertyName("code")]
    public string Code { get; init; } = string.Empty;
}
