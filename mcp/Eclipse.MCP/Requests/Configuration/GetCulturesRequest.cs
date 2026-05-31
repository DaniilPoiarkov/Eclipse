using Eclipse.MCP.Core.Client;

using System.Net.Http.Json;

namespace Eclipse.MCP.Requests.Configuration;

internal sealed class GetCulturesRequest : IRequest<CultureResponse[]>
{
    public HttpRequestMessage Build() => new(HttpMethod.Get, "/api/configuration/cultures");

    public async ValueTask<CultureResponse[]> ParseAsync(HttpContent content, CancellationToken cancellationToken = default)
        => await content.ReadFromJsonAsync<CultureResponse[]>(cancellationToken) ?? [];
}
