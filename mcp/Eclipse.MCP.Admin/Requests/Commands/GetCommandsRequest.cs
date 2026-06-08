using Eclipse.MCP.Core.Client;

using System.Net.Http.Json;

namespace Eclipse.MCP.Admin.Requests.Commands;

internal sealed class GetCommandsRequest : IRequest<CommandResponse[]>
{
    public HttpRequestMessage Build() => new(HttpMethod.Get, "/api/commands");

    public async ValueTask<CommandResponse[]> ParseAsync(HttpContent content, CancellationToken cancellationToken = default)
        => await content.ReadFromJsonAsync<CommandResponse[]>(cancellationToken) ?? [];
}
