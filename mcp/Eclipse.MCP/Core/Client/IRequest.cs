namespace Eclipse.MCP.Core.Client;

public interface IRequest<T>
{
    HttpRequestMessage Build();
    ValueTask<T> ParseAsync(HttpContent content, CancellationToken cancellationToken = default);
}
