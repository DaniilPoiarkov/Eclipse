using Eclipse.MCP.Core.Client;
using Eclipse.MCP.Core.Tools;
using Eclipse.MCP.Core;
using Eclipse.MCP.Requests.Reminders;

using ModelContextProtocol.Server;

using System.ComponentModel;

namespace Eclipse.MCP.Tools;

public sealed class ReminderTools
{
    private readonly IEclipseClient _client;

    public ReminderTools(IEclipseClient client)
    {
        _client = client;
    }

    [McpServerTool(Name = "eclipse_reminders_get_list")]
    [Description("Retrieves all reminders for the authenticated user.\n" +
        "Use when the user asks to see, list, or show their reminders.\n" +
        "Below are user input samples which show when to call this tool:\n" +
        "<sample1>Show me my reminders</sample1>\n" +
        "<sample2>What reminders do I have?</sample2>\n" +
        "<sample3>List all my reminders</sample3>\n")
    ]
    public async Task<ToolResponse<ReminderResponse[]>> GetRemindersAsync(CancellationToken cancellationToken = default)
    {
        var response = await _client.GetRemindersAsync(cancellationToken);
        return response.ToToolResponse();
    }

    [McpServerTool(Name = "eclipse_reminders_get")]
    [Description("Retrieves a specific reminder by its ID.\n" +
        "Use when the user asks about a specific reminder and provides an ID.\n" +
        "Below are user input samples which show when to call this tool:\n" +
        "<sample1>Get reminder with id 123</sample1>\n" +
        "<sample2>Show me the details of reminder 456</sample2>\n")
    ]
    public async Task<ToolResponse<ReminderResponse>> GetReminderAsync(
        [Description("The unique identifier of the reminder.")] Guid reminderId,
        CancellationToken cancellationToken = default)
    {
        var response = await _client.GetReminderAsync(reminderId, cancellationToken);
        return response.ToToolResponse();
    }

    [McpServerTool(Name = "eclipse_reminders_create")]
    [Description("Creates a new reminder for the authenticated user.\n" +
        "Use when the user wants to add or set a reminder at a specific time.\n" +
        "Below are user input samples which show when to call this tool:\n" +
        "<sample1>Remind me to call mom at 18:00</sample1>\n" +
        "<sample2>Set a reminder for the meeting at 09:30</sample2>\n" +
        "<sample3>Create a reminder: take medicine at 08:00</sample3>\n")
    ]
    public async Task<ToolResponse<ReminderResponse>> CreateReminderAsync(
        [Description("The reminder message text.")] string text,
        [Description("The time to send the reminder notification in HH:mm format (e.g. '14:30').")] string notifyAt,
        [Description("Optional ID of a related todo item to link this reminder to.")] Guid? relatedItemId = null,
        CancellationToken cancellationToken = default)
    {
        var response = await _client.CreateReminderAsync(text, notifyAt, relatedItemId, cancellationToken);
        return response.ToToolResponse();
    }
}
