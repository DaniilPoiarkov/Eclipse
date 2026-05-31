using Eclipse.MCP.Core.Client;

namespace Eclipse.MCP.Requests.Cache;

internal sealed class PruneCacheRequest : IRequest<string>
{
    public HttpRequestMessage Build() => new(HttpMethod.Delete, "/api/cache/prune");

    public ValueTask<string> ParseAsync(HttpContent content, CancellationToken cancellationToken = default)
        => ValueTask.FromResult(string.Empty);
}
