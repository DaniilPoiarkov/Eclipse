using Eclipse.MCP.Core.Client;

namespace Eclipse.MCP.Admin.Requests.Ping;

internal static class PingExtensions
{
    public static Task<EclipseResponse<string>> PingAsync(this IEclipseClient client, CancellationToken cancellationToken = default)
        => client.SendRequestAsync(new PingRequest(), cancellationToken);
}
