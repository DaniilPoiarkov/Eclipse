using Eclipse.MCP.Core.Client;
using Eclipse.MCP.Core.Tools;
using Eclipse.MCP.Requests.Cache;

using ModelContextProtocol.Server;

using System.ComponentModel;

namespace Eclipse.MCP.Tools;

public sealed class CacheTools
{
    private readonly IEclipseClient _client;

    public CacheTools(IEclipseClient client)
    {
        _client = client;
    }

    [McpServerTool(Name = "eclipse_cache_prune")]
    [Description("Prunes the application cache, removing all expired or stale entries. Requires admin privileges.\n" +
        "Use when the user wants to clear or prune the application cache.\n" +
        "Below are user input samples which show when to call this tool:\n" +
        "<sample1>Prune the cache</sample1>\n" +
        "<sample2>Clear the Eclipse app cache</sample2>\n")
    ]
    public async Task<ToolResponse<string>> PruneCacheAsync(CancellationToken cancellationToken = default)
    {
        var response = await _client.PruneCacheAsync(cancellationToken);
        return response.ToToolResponse();
    }
}
