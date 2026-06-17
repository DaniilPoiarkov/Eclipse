using Eclipse.MCP.Core.Client;

namespace Eclipse.MCP.Admin.Requests.Commands;

internal static class CommandExtensions
{
    public static Task<EclipseResponse<CommandResponse[]>> GetCommandsAsync(this IEclipseClient client, CancellationToken cancellationToken = default)
        => client.SendRequestAsync(new GetCommandsRequest(), cancellationToken);

    public static Task<EclipseResponse<string>> AddCommandAsync(this IEclipseClient client, string command, string description, CancellationToken cancellationToken = default)
        => client.SendRequestAsync(new AddCommandRequest(command, description), cancellationToken);

    public static Task<EclipseResponse<string>> RemoveCommandAsync(this IEclipseClient client, string command, CancellationToken cancellationToken = default)
        => client.SendRequestAsync(new RemoveCommandRequest(command), cancellationToken);
}
