using Eclipse.MCP.Core.Client;

using System.Net.Http.Json;

namespace Eclipse.MCP.Requests.Configuration;

internal sealed class GetMoodStateScoresRequest : IRequest<MoodStateScoreResponse[]>
{
    public HttpRequestMessage Build() => new(HttpMethod.Get, "/api/configuration/mood-state-scores");

    public async ValueTask<MoodStateScoreResponse[]> ParseAsync(HttpContent content, CancellationToken cancellationToken = default)
        => await content.ReadFromJsonAsync<MoodStateScoreResponse[]>(cancellationToken) ?? [];
}
