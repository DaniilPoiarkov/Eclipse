using Eclipse.MCP.Core.Client;
using Eclipse.MCP.Core.Tools;
using Eclipse.MCP.Requests.UserStatistics;

using ModelContextProtocol.Server;

using System.ComponentModel;

namespace Eclipse.MCP.Tools;

public sealed class UserStatisticsTools
{
    private readonly IEclipseClient _client;

    public UserStatisticsTools(IEclipseClient client)
    {
        _client = client;
    }

    [McpServerTool(Name = "eclipse_user_statistics_get")]
    [Description("Retrieves statistics for the authenticated user, including finished todo items and received reminders.\n" +
        "Use when the user asks about their activity statistics or progress.\n" +
        "Below are user input samples which show when to call this tool:\n" +
        "<sample1>Show my statistics</sample1>\n" +
        "<sample2>How many tasks have I completed?</sample2>\n" +
        "<sample3>Show my Eclipse activity</sample3>\n")
    ]
    public async Task<ToolResponse<UserStatisticsResponse>> GetUserStatisticsAsync(CancellationToken cancellationToken = default)
    {
        var response = await _client.GetUserStatisticsAsync(cancellationToken);
        return response.ToToolResponse();
    }
}
