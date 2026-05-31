namespace Eclipse.MCP.Core.Client;

internal sealed class EclipseClient : IEclipseClient
{
    private readonly HttpClient _httpClient;

    public EclipseClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<EclipseResponse<T>> SendRequestAsync<T>(IRequest<T> request, CancellationToken cancellationToken = default)
        where T : class
    {
        using var httpRequest = request.Build();
        using var response = await _httpClient.SendAsync(httpRequest, cancellationToken);

        var content = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return EclipseResponse<T>.Failure(content, response.StatusCode);
        }

        return EclipseResponse<T>.Success(
            await request.ParseAsync(response.Content, cancellationToken),
            content,
            response.StatusCode
        );
    }
}
