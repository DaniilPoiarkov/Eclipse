using Eclipse.MCP.Core.Client;

using System.Net.Http.Json;

namespace Eclipse.MCP.Requests.MoodRecords;

internal sealed class CreateMoodRecordRequest : IRequest<MoodRecordResponse>
{
    private readonly string _state;

    public CreateMoodRecordRequest(string state)
    {
        _state = state;
    }

    public HttpRequestMessage Build()
    {
        var message = new HttpRequestMessage(HttpMethod.Post, "/api/mood-records/add");
        message.Content = JsonContent.Create(new { state = _state });
        return message;
    }

    public async ValueTask<MoodRecordResponse> ParseAsync(HttpContent content, CancellationToken cancellationToken = default)
        => await content.ReadFromJsonAsync<MoodRecordResponse>(cancellationToken) ?? new MoodRecordResponse();
}
