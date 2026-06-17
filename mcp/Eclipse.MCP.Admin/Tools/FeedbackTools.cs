using Eclipse.MCP.Admin.Requests.Feedbacks;
using Eclipse.MCP.Core;
using Eclipse.MCP.Core.Client;
using Eclipse.MCP.Core.Tools;

using ModelContextProtocol.Server;

using System.ComponentModel;

namespace Eclipse.MCP.Admin.Tools;

public sealed class FeedbackTools
{
    private readonly IEclipseClient _client;

    public FeedbackTools(IEclipseClient client)
    {
        _client = client;
    }

    [McpServerTool(Name = "eclipse_feedbacks_get_list")]
    [Description("Retrieves a paginated list of user feedbacks. Requires admin privileges.\n" +
        "Use when the user wants to see feedback submitted by users.\n" +
        "Below are user input samples which show when to call this tool:\n" +
        "<sample1>Show me all user feedbacks</sample1>\n" +
        "<sample2>Get feedbacks from last month</sample2>\n")
    ]
    public async Task<ToolResponse<PaginatedFeedbackResponse>> GetFeedbacksAsync(
        [Description("The page number to retrieve (starting at 1).")] int page = 1,
        [Description("The number of items per page (max 25).")] int pageSize = 10,
        [Description("Optional start date filter in ISO 8601 format (e.g. '2024-01-01T00:00:00').")] DateTime? from = null,
        [Description("Optional end date filter in ISO 8601 format (e.g. '2024-12-31T23:59:59').")] DateTime? to = null,
        CancellationToken cancellationToken = default)
    {
        var response = await _client.GetFeedbacksAsync(page, pageSize, from, to, cancellationToken);
        return response.ToToolResponse();
    }

    [McpServerTool(Name = "eclipse_feedbacks_request")]
    [Description("Sends a feedback request to all users via Telegram. Requires admin privileges.\n" +
        "Use when the admin wants to proactively request feedback from users.\n" +
        "Below are user input samples which show when to call this tool:\n" +
        "<sample1>Request feedback from all users</sample1>\n" +
        "<sample2>Send a feedback request to users</sample2>\n")
    ]
    public async Task<ToolResponse<string>> RequestFeedbackAsync(CancellationToken cancellationToken = default)
    {
        var response = await _client.RequestFeedbackAsync(cancellationToken);
        return response.ToToolResponse();
    }
}
