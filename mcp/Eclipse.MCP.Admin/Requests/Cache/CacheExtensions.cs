using Eclipse.MCP.Core.Client;

namespace Eclipse.MCP.Admin.Requests.Cache;

internal static class CacheExtensions
{
    public static Task<EclipseResponse<string>> PruneCacheAsync(this IEclipseClient client, CancellationToken cancellationToken = default)
        => client.SendRequestAsync(new PruneCacheRequest(), cancellationToken);
}
