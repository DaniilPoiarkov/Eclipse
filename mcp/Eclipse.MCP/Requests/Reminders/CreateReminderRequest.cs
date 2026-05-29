using Eclipse.MCP.Core.Client;

using System.Net.Http.Json;

namespace Eclipse.MCP.Requests.Reminders;

internal sealed class CreateReminderRequest : IRequest<ReminderResponse>
{
    private readonly string _text;
    private readonly string _notifyAt;
    private readonly Guid? _relatedItemId;

    public CreateReminderRequest(string text, string notifyAt, Guid? relatedItemId = null)
    {
        _text = text;
        _notifyAt = notifyAt;
        _relatedItemId = relatedItemId;
    }

    public HttpRequestMessage Build()
    {
        var message = new HttpRequestMessage(HttpMethod.Post, "/api/reminders");
        message.Content = JsonContent.Create(new
        {
            relatedItemId = _relatedItemId,
            text = _text,
            notifyAt = _notifyAt
        });
        return message;
    }

    public async ValueTask<ReminderResponse> ParseAsync(HttpContent content, CancellationToken cancellationToken = default)
        => await content.ReadFromJsonAsync<ReminderResponse>(cancellationToken) ?? new ReminderResponse();
}
