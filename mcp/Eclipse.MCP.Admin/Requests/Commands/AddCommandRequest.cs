using Eclipse.MCP.Core.Client;

using System.Net.Http.Json;

namespace Eclipse.MCP.Admin.Requests.Commands;

internal sealed class AddCommandRequest : IRequest<string>
{
    private readonly string _command;
    private readonly string _description;

    public AddCommandRequest(string command, string description)
    {
        _command = command;
        _description = description;
    }

    public HttpRequestMessage Build()
    {
        var message = new HttpRequestMessage(HttpMethod.Post, "/api/commands/add");
        message.Content = JsonContent.Create(new { command = _command, description = _description });
        return message;
    }

    public ValueTask<string> ParseAsync(HttpContent content, CancellationToken cancellationToken = default)
        => ValueTask.FromResult(string.Empty);
}
