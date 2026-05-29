using Eclipse.MCP.Core.Client;

using System.Net.Http.Json;

namespace Eclipse.MCP.Requests.MoodRecords;

internal sealed class GetMoodRecordsRequest : IRequest<MoodRecordResponse[]>
{
    public HttpRequestMessage Build() => new(HttpMethod.Get, "/api/mood-records");

    public async ValueTask<MoodRecordResponse[]> ParseAsync(HttpContent content, CancellationToken cancellationToken = default)
        => await content.ReadFromJsonAsync<MoodRecordResponse[]>(cancellationToken) ?? [];
}
