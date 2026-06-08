using Eclipse.MCP.Core.Client;

namespace Eclipse.MCP.Admin.Requests.Users;

internal static class UserExtensions
{
    public static Task<EclipseResponse<PaginatedUserResponse>> GetUsersAsync(this IEclipseClient client, int page, int pageSize, string? name = null, string? userName = null, bool onlyActive = false, CancellationToken cancellationToken = default)
        => client.SendRequestAsync(new GetUserListRequest(page, pageSize, name, userName, onlyActive), cancellationToken);
}
