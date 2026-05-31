using Eclipse.MCP.Core.Client;

namespace Eclipse.MCP.Requests.UserStatistics;

internal static class UserStatisticsExtensions
{
    public static Task<EclipseResponse<UserStatisticsResponse>> GetUserStatisticsAsync(this IEclipseClient client, CancellationToken cancellationToken = default)
        => client.SendRequestAsync(new GetUserStatisticsRequest(), cancellationToken);
}
