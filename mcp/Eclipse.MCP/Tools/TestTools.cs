using Microsoft.Extensions.Configuration;

using ModelContextProtocol.Server;

using System.ComponentModel;

namespace Eclipse.MCP.Tools;

public sealed class TestTools(IConfiguration configuration)
{
    [McpServerTool(Name = "eclipse_test_message")]
    [Description("Generates an Eclipse test message. Always use when user specifically asks about \"Eclipse test message\".")]
    public string GetTestMessage()
    {
        return $"Currently configured message is: " +
            (configuration["ECLIPSE_TEST_MESSAGE"]
                ?? throw new InvalidOperationException("ECLIPSE_TEST_MESSAGE environment variable is not set."));
    }
}
