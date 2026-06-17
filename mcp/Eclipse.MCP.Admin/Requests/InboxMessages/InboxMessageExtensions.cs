using Eclipse.MCP.Core.Client;

namespace Eclipse.MCP.Admin.Requests.InboxMessages;

internal static class InboxMessageExtensions
{
    public static Task<EclipseResponse<string>> ResetFailedInboxMessagesAsync(this IEclipseClient client, CancellationToken cancellationToken = default)
        => client.SendRequestAsync(new ResetFailedInboxMessagesRequest(), cancellationToken);
}
