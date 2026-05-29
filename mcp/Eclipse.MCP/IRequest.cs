namespace Eclipse.MCP;

public interface IRequest<T>
{
    HttpRequestMessage Build();
    ValueTask<T> ParseAsync(HttpContent content, CancellationToken cancellationToken = default);
}
