using Eclipse.MCP.Core.Client;

namespace Eclipse.MCP.Admin.Requests.Feedbacks;

internal sealed class RequestFeedbackRequest : IRequest<string>
{
    public HttpRequestMessage Build() => new(HttpMethod.Put, "/api/feedbacks/request");

    public ValueTask<string> ParseAsync(HttpContent content, CancellationToken cancellationToken = default)
        => ValueTask.FromResult(string.Empty);
}
