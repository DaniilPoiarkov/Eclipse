using Eclipse.MCP.Core.Client;
using Eclipse.MCP.Core.Tools;
using Eclipse.MCP.Requests.Suggestions;

using ModelContextProtocol.Server;

using System.ComponentModel;

namespace Eclipse.MCP.Tools;

public sealed class SuggestionTools
{
    private readonly IEclipseClient _client;

    public SuggestionTools(IEclipseClient client)
    {
        _client = client;
    }

    [McpServerTool(Name = "eclipse_suggestions_get_list")]
    [Description("Retrieves all user suggestions along with submitter information. Requires admin privileges.\n" +
        "Use when the admin wants to review suggestions submitted by users.\n" +
        "Below are user input samples which show when to call this tool:\n" +
        "<sample1>Show me all user suggestions</sample1>\n" +
        "<sample2>List suggestions with user info</sample2>\n")
    ]
    public async Task<ToolResponse<SuggestionResponse[]>> GetSuggestionsAsync(CancellationToken cancellationToken = default)
    {
        var response = await _client.GetSuggestionsAsync(cancellationToken);
        return response.ToToolResponse();
    }
}
