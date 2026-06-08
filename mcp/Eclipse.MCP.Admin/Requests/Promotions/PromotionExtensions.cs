using Eclipse.MCP.Core.Client;

namespace Eclipse.MCP.Admin.Requests.Promotions;

internal static class PromotionExtensions
{
    public static Task<EclipseResponse<PromotionResponse>> FindPromotionAsync(this IEclipseClient client, Guid id, CancellationToken cancellationToken = default)
        => client.SendRequestAsync(new FindPromotionRequest(id), cancellationToken);

    public static Task<EclipseResponse<PromotionResponse>> PublishPromotionAsync(this IEclipseClient client, Guid id, CancellationToken cancellationToken = default)
        => client.SendRequestAsync(new PublishPromotionRequest(id), cancellationToken);
}
