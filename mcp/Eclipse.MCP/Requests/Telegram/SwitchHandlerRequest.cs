using Eclipse.MCP.Core.Client;

using System.Net.Http.Json;

namespace Eclipse.MCP.Requests.Telegram;

internal sealed class SwitchHandlerRequest : IRequest<string>
{
    private readonly string _type;

    public SwitchHandlerRequest(string type)
    {
        _type = type;
    }

    public HttpRequestMessage Build()
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/telegram/switch-handler");
        request.Content = JsonContent.Create(new { type = _type });
        return request;
    }

    public ValueTask<string> ParseAsync(HttpContent content, CancellationToken cancellationToken = default)
        => ValueTask.FromResult(string.Empty);
}
