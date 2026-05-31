using Eclipse.MCP.Core.Client;

namespace Eclipse.MCP.Requests.Feedbacks;

internal static class FeedbackExtensions
{
    public static Task<EclipseResponse<PaginatedFeedbackResponse>> GetFeedbacksAsync(this IEclipseClient client, int page, int pageSize, DateTime? from = null, DateTime? to = null, CancellationToken cancellationToken = default)
        => client.SendRequestAsync(new GetFeedbacksRequest(page, pageSize, from, to), cancellationToken);

    public static Task<EclipseResponse<string>> RequestFeedbackAsync(this IEclipseClient client, CancellationToken cancellationToken = default)
        => client.SendRequestAsync(new RequestFeedbackRequest(), cancellationToken);
}
