using Eclipse.MCP.Core.Client;

using System.Net.Http.Json;

namespace Eclipse.MCP.Requests.MoodRecords;

internal sealed class GetMoodRecordRequest : IRequest<MoodRecordResponse>
{
    private readonly Guid _moodRecordId;

    public GetMoodRecordRequest(Guid moodRecordId)
    {
        _moodRecordId = moodRecordId;
    }

    public HttpRequestMessage Build() => new(HttpMethod.Get, $"/api/mood-records/{_moodRecordId}");

    public async ValueTask<MoodRecordResponse> ParseAsync(HttpContent content, CancellationToken cancellationToken = default)
        => await content.ReadFromJsonAsync<MoodRecordResponse>(cancellationToken) ?? new MoodRecordResponse();
}
