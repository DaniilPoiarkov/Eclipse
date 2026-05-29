using Eclipse.MCP.Core.Client;

using System.Net.Http.Json;

namespace Eclipse.MCP.Requests.Telegram;

internal sealed class SendTelegramMessageRequest : IRequest<string>
{
    private readonly string _message;
    private readonly long _chatId;

    public SendTelegramMessageRequest(string message, long chatId)
    {
        _message = message;
        _chatId = chatId;
    }

    public HttpRequestMessage Build()
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/telegram/send");
        request.Content = JsonContent.Create(new { message = _message, chatId = _chatId });
        return request;
    }

    public ValueTask<string> ParseAsync(HttpContent content, CancellationToken cancellationToken = default)
        => ValueTask.FromResult(string.Empty);
}
