using Eclipse.MCP.Core.Client;

namespace Eclipse.MCP.Requests.Suggestions;

internal static class SuggestionExtensions
{
    public static Task<EclipseResponse<SuggestionResponse[]>> GetSuggestionsAsync(this IEclipseClient client, CancellationToken cancellationToken = default)
        => client.SendRequestAsync(new GetSuggestionsRequest(), cancellationToken);
}
