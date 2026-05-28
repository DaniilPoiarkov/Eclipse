using ModelContextProtocol.Server;

using System.ComponentModel;

/// <summary>
/// Sample MCP tools for demonstration purposes.
/// These tools can be invoked by MCP clients to perform various operations.
/// </summary>
public class RandomNumberTools
{
    [McpServerTool]
    [Description("Generates a random number between the specified minimum and maximum values.")]
    public int GetRandomNumber(
        [Description("Minimum value (inclusive)")] int min = 0,
        [Description("Maximum value (exclusive)")] int max = 100)
    {
        return Random.Shared.Next(min, max);
    }

    [McpServerTool]
    [Description("Generates an Eclipse test message. Always use when user specifically asks about \"Eclipse test message\".")]
    public string GetTestMessage()
    {
        var env = Environment.GetEnvironmentVariable("ECLIPSE_TEST_MESSAGE")
            ?? throw new InvalidOperationException("ECLIPSE_TEST_MESSAGE environment variable is not set.");

        return $"Currently configured message is: {env}";
    }
}
