using System.Text.Json.Serialization;

namespace Eclipse.MCP.Admin.Requests.Commands;

public sealed class CommandResponse
{
    [JsonPropertyName("command")]
    public string Command { get; init; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; init; } = string.Empty;
}
