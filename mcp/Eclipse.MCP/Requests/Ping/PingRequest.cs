namespace Eclipse.MCP.Requests.Ping;

internal sealed class PingRequest : IRequest<string>
{
    public HttpRequestMessage Build() => new(HttpMethod.Get, "/api/v2/ping");

    public async ValueTask<string> ParseAsync(HttpContent content, CancellationToken cancellationToken)
        => await content.ReadAsStringAsync(cancellationToken);
}
