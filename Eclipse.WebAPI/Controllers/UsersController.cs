using Eclipse.Application.Contracts.IdentityUsers;
using Eclipse.Common.Linq;
using Eclipse.WebAPI.Filters.Authorization;

using Microsoft.AspNetCore.Mvc;

namespace Eclipse.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
[ApiKeyAuthorize]
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
        return Ok(await _service.GetAllAsync(cancellationToken));
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
