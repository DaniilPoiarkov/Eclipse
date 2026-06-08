using Eclipse.MCP.Admin.Requests.InboxMessages;
using Eclipse.MCP.Core.Client;
using Eclipse.MCP.Core.Tools;

using ModelContextProtocol.Server;

using System.ComponentModel;

namespace Eclipse.MCP.Admin.Tools;

public sealed class InboxMessageTools
{
    private readonly IEclipseClient _client;

    public InboxMessageTools(IEclipseClient client)
    {
        _client = client;
    }

    [McpServerTool(Name = "eclipse_inbox_messages_reset_failed")]
    [Description("Resets all failed inbox messages so they can be reprocessed. Requires admin privileges.\n" +
        "Use when the admin wants to retry processing inbox messages that previously failed.\n" +
        "Below are user input samples which show when to call this tool:\n" +
        "<sample1>Reset failed inbox messages</sample1>\n" +
        "<sample2>Retry failed inbox messages</sample2>\n")
    ]
    public async Task<ToolResponse<string>> ResetFailedInboxMessagesAsync(CancellationToken cancellationToken = default)
    {
        var response = await _client.ResetFailedInboxMessagesAsync(cancellationToken);
        return response.ToToolResponse();
    }
}
