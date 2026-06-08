using Eclipse.MCP.Admin.Requests.Telegram;
using Eclipse.MCP.Core.Client;
using Eclipse.MCP.Core.Tools;

using ModelContextProtocol.Server;

using System.ComponentModel;

namespace Eclipse.MCP.Admin.Tools;

public sealed class TelegramTools
{
    private readonly IEclipseClient _client;

    public TelegramTools(IEclipseClient client)
    {
        _client = client;
    }

    [McpServerTool(Name = "eclipse_telegram_send")]
    [Description("Sends a Telegram message to a specific chat. Requires admin privileges.\n" +
        "Use when the admin wants to send a direct message to a user's Telegram chat.\n" +
        "Below are user input samples which show when to call this tool:\n" +
        "<sample1>Send 'Hello!' to chat 123456789</sample1>\n" +
        "<sample2>Message the user with chat id 987654321: 'Your request was approved'</sample2>\n")
    ]
    public async Task<ToolResponse<string>> SendMessageAsync(
        [Description("The message text to send.")] string message,
        [Description("The Telegram chat ID of the recipient.")] long chatId,
        CancellationToken cancellationToken = default)
    {
        var response = await _client.SendTelegramMessageAsync(message, chatId, cancellationToken);
        return response.ToToolResponse();
    }

    [McpServerTool(Name = "eclipse_telegram_switch_handler")]
    [Description("Switches the active Telegram webhook handler type. Requires admin privileges.\n" +
        "Valid handler types: Active, Disabled.\n" +
        "Use when the admin wants to enable or disable the bot's message processing.\n" +
        "Below are user input samples which show when to call this tool:\n" +
        "<sample1>Switch the handler to Active</sample1>\n" +
        "<sample2>Disable the Telegram handler</sample2>\n")
    ]
    public async Task<ToolResponse<string>> SwitchHandlerAsync(
        [Description("The handler type to switch to. Valid values: Active, Disabled.")] string type,
        CancellationToken cancellationToken = default)
    {
        var response = await _client.SwitchHandlerAsync(type, cancellationToken);
        return response.ToToolResponse();
    }
}
