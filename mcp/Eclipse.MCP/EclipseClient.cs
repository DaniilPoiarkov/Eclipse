namespace Eclipse.MCP;

internal sealed class EclipseClient : IEclipseClient
{
    private readonly HttpClient _httpClient;

    public EclipseClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<T> SendRequestAsync<T>(IRequest<T> request, CancellationToken cancellationToken = default)
    {
        using var httpRequest = request.Build();
        using var response = await _httpClient.SendAsync(httpRequest, cancellationToken);

        response.EnsureSuccessStatusCode();

        return await request.ParseAsync(response.Content, cancellationToken);
    }
}
