using Eclipse.MCP.Core.Client;

using System.Net.Http.Json;

namespace Eclipse.MCP.Admin.Requests.Promotions;

internal sealed class PublishPromotionRequest : IRequest<PromotionResponse>
{
    private readonly Guid _id;

    public PublishPromotionRequest(Guid id)
    {
        _id = id;
    }

    public HttpRequestMessage Build() => new(HttpMethod.Post, $"/api/promotions/{_id}/publish");

    public async ValueTask<PromotionResponse> ParseAsync(HttpContent content, CancellationToken cancellationToken = default)
        => await content.ReadFromJsonAsync<PromotionResponse>(cancellationToken) ?? new PromotionResponse();
}
