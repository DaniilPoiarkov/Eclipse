using Eclipse.MCP.Core.Client;
using Eclipse.MCP.Core.Tools;
using Eclipse.MCP.Requests.MoodRecords;

using ModelContextProtocol.Server;

using System.ComponentModel;

namespace Eclipse.MCP.Tools;

public sealed class MoodRecordTools
{
    private readonly IEclipseClient _client;

    public MoodRecordTools(IEclipseClient client)
    {
        _client = client;
    }

    [McpServerTool(Name = "eclipse_mood_records_get_list")]
    [Description("Retrieves all mood records for the authenticated user.\n" +
        "Use when the user asks to see, list, or show their mood records or mood history.\n" +
        "Below are user input samples which show when to call this tool:\n" +
        "<sample1>Show me my mood records</sample1>\n" +
        "<sample2>What is my mood history?</sample2>\n" +
        "<sample3>List all my mood entries</sample3>\n")
    ]
    public async Task<ToolResponse<MoodRecordResponse[]>> GetMoodRecordsAsync(CancellationToken cancellationToken = default)
    {
        var response = await _client.GetMoodRecordsAsync(cancellationToken);
        return response.ToToolResponse();
    }

    [McpServerTool(Name = "eclipse_mood_records_get")]
    [Description("Retrieves a specific mood record by its ID.\n" +
        "Use when the user asks about a specific mood record and provides an ID.\n" +
        "Below are user input samples which show when to call this tool:\n" +
        "<sample1>Get mood record with id 123</sample1>\n" +
        "<sample2>Show me mood record 456</sample2>\n")
    ]
    public async Task<ToolResponse<MoodRecordResponse>> GetMoodRecordAsync(
        [Description("The unique identifier of the mood record.")] Guid moodRecordId,
        CancellationToken cancellationToken = default)
    {
        var response = await _client.GetMoodRecordAsync(moodRecordId, cancellationToken);
        return response.ToToolResponse();
    }

    [McpServerTool(Name = "eclipse_mood_records_add")]
    [Description("Creates or updates the mood record for the current day.\n" +
        "Use when the user wants to log, record, or update their current mood.\n" +
        "Valid mood states: Amazing, Excellent, VeryGood, Good, Fine, Neutral, Poor, Bad, VeryBad, Terrible.\n" +
        "Below are user input samples which show when to call this tool:\n" +
        "<sample1>Log my mood as Good</sample1>\n" +
        "<sample2>I'm feeling Amazing today, record it</sample2>\n" +
        "<sample3>Set today's mood to Neutral</sample3>\n")
    ]
    public async Task<ToolResponse<MoodRecordResponse>> AddMoodRecordAsync(
        [Description("The mood state to record. Valid values: Amazing, Excellent, VeryGood, Good, Fine, Neutral, Poor, Bad, VeryBad, Terrible.")] string state,
        CancellationToken cancellationToken = default)
    {
        var response = await _client.CreateMoodRecordAsync(state, cancellationToken);
        return response.ToToolResponse();
    }
}
