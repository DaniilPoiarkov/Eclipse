using Eclipse.MCP.Core.Client;

namespace Eclipse.MCP.Admin.Requests.Health;

internal static class HealthExtensions
{
    public static Task<EclipseResponse<HealthResponse>> HealthAsync(this IEclipseClient client, CancellationToken cancellationToken = default)
        => client.SendRequestAsync(new HealthRequest(), cancellationToken);
}
