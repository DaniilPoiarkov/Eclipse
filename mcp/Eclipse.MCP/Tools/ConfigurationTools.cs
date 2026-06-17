using Eclipse.MCP.Core.Client;
using Eclipse.MCP.Core.Tools;
using Eclipse.MCP.Core;
using Eclipse.MCP.Requests.Configuration;

using ModelContextProtocol.Server;

using System.ComponentModel;

namespace Eclipse.MCP.Tools;

public sealed class ConfigurationTools
{
    private readonly IEclipseClient _client;

    public ConfigurationTools(IEclipseClient client)
    {
        _client = client;
    }

    [McpServerTool(Name = "eclipse_configuration_get_cultures")]
    [Description("Retrieves the list of supported cultures and their locale codes.\n" +
        "Use when the user asks about available languages or supported cultures in the app.\n" +
        "Below are user input samples which show when to call this tool:\n" +
        "<sample1>What languages does Eclipse support?</sample1>\n" +
        "<sample2>Get the list of available cultures</sample2>\n")
    ]
    public async Task<ToolResponse<CultureResponse[]>> GetCulturesAsync(CancellationToken cancellationToken = default)
    {
        var response = await _client.GetCulturesAsync(cancellationToken);
        return response.ToToolResponse();
    }

    [McpServerTool(Name = "eclipse_configuration_get_mood_state_scores")]
    [Description("Retrieves the mapping of all mood states to their integer scores (Amazing=10 … Terrible=1).\n" +
        "Call this before analyzing or displaying mood records to understand what each state's score means.\n" +
        "Below are user input samples which show when to call this tool:\n" +
        "<sample1>What score does Amazing mood have?</sample1>\n" +
        "<sample2>Show me the mood state scores</sample2>\n")
    ]
    public async Task<ToolResponse<MoodStateScoreResponse[]>> GetMoodStateScoresAsync(CancellationToken cancellationToken = default)
    {
        var response = await _client.GetMoodStateScoresAsync(cancellationToken);
        return response.ToToolResponse();
    }
}
