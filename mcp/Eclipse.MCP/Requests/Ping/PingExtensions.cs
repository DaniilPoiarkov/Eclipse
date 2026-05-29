namespace Eclipse.MCP.Requests.Ping;

internal static class PingExtensions
{
    public static Task<EclipseResponse<string>> PingAsync(this IEclipseClient client, CancellationToken cancellationToken = default)
        => client.SendRequestAsync(new PingRequest(), cancellationToken);
}
