using Eclipse.MCP.Core.Client;

using System.Net.Http.Json;

namespace Eclipse.MCP.Requests.Health;

internal sealed class HealthRequest : IRequest<HealthResponse>
{
    public HttpRequestMessage Build() => new(HttpMethod.Get, "/_health-checks");

    public async ValueTask<HealthResponse> ParseAsync(HttpContent content, CancellationToken cancellationToken = default)
        => await content.ReadFromJsonAsync<HealthResponse>(cancellationToken) ?? new HealthResponse();
}
