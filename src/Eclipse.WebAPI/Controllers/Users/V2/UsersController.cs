using Asp.Versioning;

using Eclipse.Application.Contracts.IdentityUsers;
using Eclipse.Common.Linq;
using Eclipse.WebAPI.Constants;
using Eclipse.WebAPI.Filters.Authorization;

using Microsoft.AspNetCore.Mvc;

namespace Eclipse.WebAPI.Controllers.Users.V2;

[ApiController]
[ApiKeyAuthorize]
[Route("api/v{v:apiVersion}/[controller]")]
[ApiVersion(ApiVersions.V2.Version, Deprecated = ApiVersions.V2.Deprecated)]
public sealed class UsersController : ControllerBase
{
    private readonly IIdentityUserService _service;

    public UsersController(IIdentityUserService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var options = new PaginationRequest<GetUsersRequest>
        {
            Page = 1,
            PageSize = 10
        };

        return Ok(await _service.GetPaginatedListAsync(options, cancellationToken));
    }

    [HttpPost]
    public async Task<IActionResult> GetFilteredList([FromBody] GetUsersRequest request, CancellationToken cancellationToken)
    {
        return Ok(await _service.GetFilteredListAsync(request, cancellationToken));
    }

    [HttpPost("paginated")]
    public async Task<IActionResult> GetPaginatedList([FromBody] PaginationRequest<GetUsersRequest> request, CancellationToken cancellationToken)
    {
        return Ok(await _service.GetPaginatedListAsync(request, cancellationToken));
    }
}
