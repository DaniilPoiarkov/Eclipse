namespace Eclipse.MCP.Core.Client;

public interface IEclipseClient
{
    Task<EclipseResponse<T>> SendRequestAsync<T>(IRequest<T> request, CancellationToken cancellationToken = default)
        where T : class;
}
