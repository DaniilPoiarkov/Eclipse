using Eclipse.MCP.Core.Client;

using System.Net.Http.Json;

namespace Eclipse.MCP.Requests.Reminders;

internal sealed class GetRemindersRequest : IRequest<ReminderResponse[]>
{
    public HttpRequestMessage Build() => new(HttpMethod.Get, "/api/reminders");

    public async ValueTask<ReminderResponse[]> ParseAsync(HttpContent content, CancellationToken cancellationToken = default)
        => await content.ReadFromJsonAsync<ReminderResponse[]>(cancellationToken) ?? [];
}
