using Eclipse.MCP.Core.Client;

using System.Net.Http.Json;

namespace Eclipse.MCP.Admin.Requests.Suggestions;

internal sealed class GetSuggestionsRequest : IRequest<SuggestionResponse[]>
{
    public HttpRequestMessage Build() => new(HttpMethod.Get, "/api/suggestions");

    public async ValueTask<SuggestionResponse[]> ParseAsync(HttpContent content, CancellationToken cancellationToken = default)
        => await content.ReadFromJsonAsync<SuggestionResponse[]>(cancellationToken) ?? [];
}
