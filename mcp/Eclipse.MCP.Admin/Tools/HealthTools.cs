using Eclipse.MCP.Admin.Requests.Health;
using Eclipse.MCP.Core.Client;
using Eclipse.MCP.Core.Tools;

using ModelContextProtocol.Server;

using System.ComponentModel;

namespace Eclipse.MCP.Admin.Tools;

public sealed class HealthTools
{
    private readonly IEclipseClient _client;

    public HealthTools(IEclipseClient client)
    {
        _client = client;
    }

    [McpServerTool(Name = "eclipse_health")]
    [Description("Calls the health check endpoint for Eclipse app to check application health status.\n" +
        "Use when user asks about the health or status of the Eclipse application.\n" +
        "Below are user input samples which show when to call this tool:\n" +
        "<sample1>What is the health status of Eclipse?</sample1>\n" +
        "<sample2>Is Eclipse healthy?</sample2>\n" +
        "<sample3>Check Eclipse app health</sample3>\n")
    ]
    public async Task<ToolResponse<HealthResponse>> HealthAsync(CancellationToken cancellationToken = default)
    {
        var response = await _client.HealthAsync(cancellationToken);
        return response.ToToolResponse();
    }
}
