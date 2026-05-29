using ModelContextProtocol.Server;

using System.ComponentModel;

namespace Eclipse.MCP.Tools;

public sealed class PingTools(IHttpClientFactory httpClientFactory)
{
    [McpServerTool(Name = "eclipse_ping")]
    [Description("Calls ping request for Eclipse app to check minimal availability.\n" +
        "Use when user asks to ping or check whether Eclipse application is running.\n" +
        "Below are user input samples which show when to call this tool:\n" +
        "<sample1>Can you ping the eclipse app?</sample1>\n" +
        "<sample2>Is the Eclipse app running?</sample2>\n" +
        "<sample3>Ping Eclipse pls</sample3>\n")
    ]
    public async Task<PingResponse> PingAsync()
    {
        using var httpClient = httpClientFactory.CreateClient("eclipse");
        var response = await httpClient.GetAsync("/api/v2/ping");
        var content = await response.Content.ReadAsStringAsync();

        return response.IsSuccessStatusCode
            ? new PingResponse(content, false)
            : new PingResponse($"Error with status {response.StatusCode}: {content}", true);
    }
}
