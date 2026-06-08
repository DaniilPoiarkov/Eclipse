using Eclipse.MCP.Core.Client;

using System.Net.Http.Json;
using System.Text;

namespace Eclipse.MCP.Admin.Requests.Users;

internal sealed class GetUserListRequest : IRequest<PaginatedUserResponse>
{
    private readonly int _page;
    private readonly int _pageSize;
    private readonly string? _name;
    private readonly string? _userName;
    private readonly bool _onlyActive;

    public GetUserListRequest(int page, int pageSize, string? name = null, string? userName = null, bool onlyActive = false)
    {
        _page = page;
        _pageSize = pageSize;
        _name = name;
        _userName = userName;
        _onlyActive = onlyActive;
    }

    public HttpRequestMessage Build()
    {
        var url = new StringBuilder($"/api/v2/users?Page={_page}&PageSize={_pageSize}&Options.OnlyActive={_onlyActive}");

        if (_name is not null)
            url.Append($"&Options.Name={Uri.EscapeDataString(_name)}");

        if (_userName is not null)
            url.Append($"&Options.UserName={Uri.EscapeDataString(_userName)}");

        return new HttpRequestMessage(HttpMethod.Get, url.ToString());
    }

    public async ValueTask<PaginatedUserResponse> ParseAsync(HttpContent content, CancellationToken cancellationToken = default)
        => await content.ReadFromJsonAsync<PaginatedUserResponse>(cancellationToken) ?? new PaginatedUserResponse();
}
