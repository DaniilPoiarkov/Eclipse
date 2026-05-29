using Eclipse.MCP.Tools;

namespace Eclipse.MCP;

internal sealed class EclipseClient(HttpClient httpClient) : IEclipseClient
{
    public async Task<PingResponse> PingAsync()
    {
        var response = await httpClient.GetAsync("/api/v2/ping");
        var content = await response.Content.ReadAsStringAsync();

        return response.IsSuccessStatusCode
            ? new PingResponse(content, false)
            : new PingResponse($"Error with status {response.StatusCode}: {content}", true);
    }
}
