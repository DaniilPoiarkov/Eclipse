using Eclipse.MCP.Core.Client;

using System.Net.Http.Json;

namespace Eclipse.MCP.Requests.UserStatistics;

internal sealed class GetUserStatisticsRequest : IRequest<UserStatisticsResponse>
{
    public HttpRequestMessage Build() => new(HttpMethod.Get, "/api/user-statistics");

    public async ValueTask<UserStatisticsResponse> ParseAsync(HttpContent content, CancellationToken cancellationToken = default)
        => await content.ReadFromJsonAsync<UserStatisticsResponse>(cancellationToken) ?? new UserStatisticsResponse();
}
