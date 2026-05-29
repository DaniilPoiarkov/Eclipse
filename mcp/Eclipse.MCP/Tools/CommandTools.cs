using Eclipse.MCP.Core.Client;
using Eclipse.MCP.Core.Tools;
using Eclipse.MCP.Requests.Commands;

using ModelContextProtocol.Server;

using System.ComponentModel;

namespace Eclipse.MCP.Tools;

public sealed class CommandTools
{
    private readonly IEclipseClient _client;

    public CommandTools(IEclipseClient client)
    {
        _client = client;
    }

    [McpServerTool(Name = "eclipse_commands_get_list")]
    [Description("Retrieves all registered Telegram bot commands.\n" +
        "Use when the user wants to see all available bot commands.\n" +
        "Below are user input samples which show when to call this tool:\n" +
        "<sample1>Show all bot commands</sample1>\n" +
        "<sample2>List the Telegram commands</sample2>\n")
    ]
    public async Task<ToolResponse<CommandResponse[]>> GetCommandsAsync(CancellationToken cancellationToken = default)
    {
        var response = await _client.GetCommandsAsync(cancellationToken);
        return response.ToToolResponse();
    }

    [McpServerTool(Name = "eclipse_commands_add")]
    [Description("Registers a new Telegram bot command. Requires admin privileges.\n" +
        "Use when the user wants to add or register a new bot command.\n" +
        "Below are user input samples which show when to call this tool:\n" +
        "<sample1>Add bot command /start with description 'Start the bot'</sample1>\n" +
        "<sample2>Register new command /help - Shows help menu</sample2>\n")
    ]
    public async Task<ToolResponse<string>> AddCommandAsync(
        [Description("The command name including the leading slash (e.g. '/start').")] string command,
        [Description("A short description of what the command does.")] string description,
        CancellationToken cancellationToken = default)
    {
        var response = await _client.AddCommandAsync(command, description, cancellationToken);
        return response.ToToolResponse();
    }

    [McpServerTool(Name = "eclipse_commands_remove")]
    [Description("Removes a registered Telegram bot command. Requires admin privileges.\n" +
        "Use when the user wants to delete or unregister a bot command.\n" +
        "Below are user input samples which show when to call this tool:\n" +
        "<sample1>Remove the /start command</sample1>\n" +
        "<sample2>Delete bot command /help</sample2>\n")
    ]
    public async Task<ToolResponse<string>> RemoveCommandAsync(
        [Description("The command name to remove (e.g. '/start').")] string command,
        CancellationToken cancellationToken = default)
    {
        var response = await _client.RemoveCommandAsync(command, cancellationToken);
        return response.ToToolResponse();
    }
}
