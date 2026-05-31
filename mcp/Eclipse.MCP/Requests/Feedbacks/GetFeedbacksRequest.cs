using Eclipse.MCP.Core.Client;

using System.Net.Http.Json;
using System.Text;

namespace Eclipse.MCP.Requests.Feedbacks;

internal sealed class GetFeedbacksRequest : IRequest<PaginatedFeedbackResponse>
{
    private readonly int _page;
    private readonly int _pageSize;
    private readonly DateTime? _from;
    private readonly DateTime? _to;

    public GetFeedbacksRequest(int page, int pageSize, DateTime? from = null, DateTime? to = null)
    {
        _page = page;
        _pageSize = pageSize;
        _from = from;
        _to = to;
    }

    public HttpRequestMessage Build()
    {
        var url = new StringBuilder($"/api/feedbacks?Page={_page}&PageSize={_pageSize}");

        if (_from.HasValue)
            url.Append($"&Options.From={_from.Value:O}");

        if (_to.HasValue)
            url.Append($"&Options.To={_to.Value:O}");

        return new HttpRequestMessage(HttpMethod.Get, url.ToString());
    }

    public async ValueTask<PaginatedFeedbackResponse> ParseAsync(HttpContent content, CancellationToken cancellationToken = default)
        => await content.ReadFromJsonAsync<PaginatedFeedbackResponse>(cancellationToken) ?? new PaginatedFeedbackResponse();
}
