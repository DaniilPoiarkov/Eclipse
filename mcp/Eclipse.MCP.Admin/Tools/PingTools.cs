using Eclipse.MCP.Admin.Requests.Ping;
using Eclipse.MCP.Core.Client;
using Eclipse.MCP.Core.Tools;

using ModelContextProtocol.Server;

using System.ComponentModel;

namespace Eclipse.MCP.Admin.Tools;

public sealed class PingTools
{
    private readonly IEclipseClient _client;

    public PingTools(IEclipseClient client)
    {
        _client = client;
    }

    [McpServerTool(Name = "eclipse_ping")]
    [Description("Calls ping request for Eclipse app to check minimal availability.\n" +
        "Use when user asks to ping or check whether Eclipse application is running.\n" +
        "Below are user input samples which show when to call this tool:\n" +
        "<sample1>Can you ping the eclipse app?</sample1>\n" +
        "<sample2>Is the Eclipse app running?</sample2>\n" +
        "<sample3>Ping Eclipse pls</sample3>\n")
    ]
    public async Task<ToolResponse<string>> PingAsync(CancellationToken cancellationToken = default)
    {
        var response = await _client.PingAsync(cancellationToken);
        return response.ToToolResponse();
    }
}
