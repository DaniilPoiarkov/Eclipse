using Eclipse.MCP.Admin.Requests.Promotions;
using Eclipse.MCP.Core.Client;
using Eclipse.MCP.Core.Tools;

using ModelContextProtocol.Server;

using System.ComponentModel;

namespace Eclipse.MCP.Admin.Tools;

public sealed class PromotionTools
{
    private readonly IEclipseClient _client;

    public PromotionTools(IEclipseClient client)
    {
        _client = client;
    }

    [McpServerTool(Name = "eclipse_promotions_find")]
    [Description("Retrieves a promotion by its ID. Requires admin privileges.\n" +
        "Use when the user wants to look up details of a specific promotion.\n" +
        "Below are user input samples which show when to call this tool:\n" +
        "<sample1>Find promotion with id 123e4567-e89b-12d3-a456-426614174000</sample1>\n" +
        "<sample2>Show me promotion 456</sample2>\n")
    ]
    public async Task<ToolResponse<PromotionResponse>> FindPromotionAsync(
        [Description("The unique identifier of the promotion.")] Guid id,
        CancellationToken cancellationToken = default)
    {
        var response = await _client.FindPromotionAsync(id, cancellationToken);
        return response.ToToolResponse();
    }

    [McpServerTool(Name = "eclipse_promotions_publish")]
    [Description("Publishes a promotion, making it visible to users. Requires admin privileges.\n" +
        "Use when the admin wants to publish or activate a pending promotion.\n" +
        "Below are user input samples which show when to call this tool:\n" +
        "<sample1>Publish promotion 123e4567-e89b-12d3-a456-426614174000</sample1>\n" +
        "<sample2>Activate and publish promotion 456</sample2>\n")
    ]
    public async Task<ToolResponse<PromotionResponse>> PublishPromotionAsync(
        [Description("The unique identifier of the promotion to publish.")] Guid id,
        CancellationToken cancellationToken = default)
    {
        var response = await _client.PublishPromotionAsync(id, cancellationToken);
        return response.ToToolResponse();
    }
}
