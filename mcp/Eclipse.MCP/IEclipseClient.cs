namespace Eclipse.MCP;

public interface IEclipseClient
{
    Task<T> SendRequestAsync<T>(IRequest<T> request, CancellationToken cancellationToken = default);
}
