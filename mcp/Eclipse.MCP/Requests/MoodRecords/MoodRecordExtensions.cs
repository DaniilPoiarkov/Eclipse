using Eclipse.MCP.Core.Client;

namespace Eclipse.MCP.Requests.MoodRecords;

internal static class MoodRecordExtensions
{
    public static Task<EclipseResponse<MoodRecordResponse[]>> GetMoodRecordsAsync(this IEclipseClient client, CancellationToken cancellationToken = default)
        => client.SendRequestAsync(new GetMoodRecordsRequest(), cancellationToken);

    public static Task<EclipseResponse<MoodRecordResponse>> GetMoodRecordAsync(this IEclipseClient client, Guid moodRecordId, CancellationToken cancellationToken = default)
        => client.SendRequestAsync(new GetMoodRecordRequest(moodRecordId), cancellationToken);

    public static Task<EclipseResponse<MoodRecordResponse>> CreateMoodRecordAsync(this IEclipseClient client, string state, CancellationToken cancellationToken = default)
        => client.SendRequestAsync(new CreateMoodRecordRequest(state), cancellationToken);
}
