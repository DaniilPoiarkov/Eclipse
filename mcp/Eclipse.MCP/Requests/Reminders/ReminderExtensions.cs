using Eclipse.MCP.Core.Client;

namespace Eclipse.MCP.Requests.Reminders;

internal static class ReminderExtensions
{
    public static Task<EclipseResponse<ReminderResponse[]>> GetRemindersAsync(this IEclipseClient client, CancellationToken cancellationToken = default)
        => client.SendRequestAsync(new GetRemindersRequest(), cancellationToken);

    public static Task<EclipseResponse<ReminderResponse>> GetReminderAsync(this IEclipseClient client, Guid reminderId, CancellationToken cancellationToken = default)
        => client.SendRequestAsync(new GetReminderRequest(reminderId), cancellationToken);

    public static Task<EclipseResponse<ReminderResponse>> CreateReminderAsync(this IEclipseClient client, string text, string notifyAt, Guid? relatedItemId = null, CancellationToken cancellationToken = default)
        => client.SendRequestAsync(new CreateReminderRequest(text, notifyAt, relatedItemId), cancellationToken);
}
