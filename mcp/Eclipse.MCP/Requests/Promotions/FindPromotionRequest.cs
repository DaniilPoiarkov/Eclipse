using Eclipse.MCP.Core.Client;

using System.Net.Http.Json;

namespace Eclipse.MCP.Requests.Promotions;

internal sealed class FindPromotionRequest : IRequest<PromotionResponse>
{
    private readonly Guid _id;

    public FindPromotionRequest(Guid id)
    {
        _id = id;
    }

    public HttpRequestMessage Build() => new(HttpMethod.Get, $"/api/promotions/{_id}");

    public async ValueTask<PromotionResponse> ParseAsync(HttpContent content, CancellationToken cancellationToken = default)
        => await content.ReadFromJsonAsync<PromotionResponse>(cancellationToken) ?? new PromotionResponse();
}
