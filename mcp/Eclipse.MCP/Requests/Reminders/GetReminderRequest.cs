using Eclipse.MCP.Core.Client;

using System.Net.Http.Json;

namespace Eclipse.MCP.Requests.Reminders;

internal sealed class GetReminderRequest : IRequest<ReminderResponse>
{
    private readonly Guid _reminderId;

    public GetReminderRequest(Guid reminderId)
    {
        _reminderId = reminderId;
    }

    public HttpRequestMessage Build() => new(HttpMethod.Get, $"/api/reminders/{_reminderId}");

    public async ValueTask<ReminderResponse> ParseAsync(HttpContent content, CancellationToken cancellationToken = default)
        => await content.ReadFromJsonAsync<ReminderResponse>(cancellationToken) ?? new ReminderResponse();
}
