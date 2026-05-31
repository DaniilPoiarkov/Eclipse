using Eclipse.MCP.Core.Client;

namespace Eclipse.MCP.Requests.Configuration;

internal static class ConfigurationExtensions
{
    public static Task<EclipseResponse<CultureResponse[]>> GetCulturesAsync(this IEclipseClient client, CancellationToken cancellationToken = default)
        => client.SendRequestAsync(new GetCulturesRequest(), cancellationToken);
}
