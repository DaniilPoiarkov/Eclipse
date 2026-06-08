using Eclipse.MCP.Core.Client;

namespace Eclipse.MCP.Admin.Requests.Commands;

internal sealed class RemoveCommandRequest : IRequest<string>
{
    private readonly string _command;

    public RemoveCommandRequest(string command)
    {
        _command = command;
    }

    public HttpRequestMessage Build() => new(HttpMethod.Delete, $"/api/commands/{Uri.EscapeDataString(_command)}/remove");

    public ValueTask<string> ParseAsync(HttpContent content, CancellationToken cancellationToken = default)
        => ValueTask.FromResult(string.Empty);
}
