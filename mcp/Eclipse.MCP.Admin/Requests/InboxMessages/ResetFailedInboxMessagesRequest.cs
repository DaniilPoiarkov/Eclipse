using Eclipse.MCP.Core.Client;

namespace Eclipse.MCP.Admin.Requests.InboxMessages;

internal sealed class ResetFailedInboxMessagesRequest : IRequest<string>
{
    public HttpRequestMessage Build() => new(HttpMethod.Post, "/api/inbox-messages/failed/reset");

    public ValueTask<string> ParseAsync(HttpContent content, CancellationToken cancellationToken = default)
        => ValueTask.FromResult(string.Empty);
}
