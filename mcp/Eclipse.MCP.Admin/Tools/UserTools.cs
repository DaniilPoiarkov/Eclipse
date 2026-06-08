using Eclipse.MCP.Admin.Requests.Users;
using Eclipse.MCP.Core.Client;
using Eclipse.MCP.Core.Tools;

using ModelContextProtocol.Server;

using System.ComponentModel;

namespace Eclipse.MCP.Admin.Tools;

public sealed class UserTools
{
    private readonly IEclipseClient _client;

    public UserTools(IEclipseClient client)
    {
        _client = client;
    }

    [McpServerTool(Name = "eclipse_users_get_list")]
    [Description("Retrieves a paginated list of users with optional filters. Requires admin privileges.\n" +
        "Use when the admin wants to search or browse registered users.\n" +
        "Below are user input samples which show when to call this tool:\n" +
        "<sample1>Show all users</sample1>\n" +
        "<sample2>Find users named John</sample2>\n" +
        "<sample3>List active users, page 2</sample3>\n")
    ]
    public async Task<ToolResponse<PaginatedUserResponse>> GetUsersAsync(
        [Description("The page number to retrieve (starting at 1).")] int page = 1,
        [Description("The number of items per page (max 25).")] int pageSize = 10,
        [Description("Optional filter by user's first name.")] string? name = null,
        [Description("Optional filter by Telegram username.")] string? userName = null,
        [Description("When true, only returns users with an active (enabled) account.")] bool onlyActive = false,
        CancellationToken cancellationToken = default)
    {
        var response = await _client.GetUsersAsync(page, pageSize, name, userName, onlyActive, cancellationToken);
        return response.ToToolResponse();
    }
}
