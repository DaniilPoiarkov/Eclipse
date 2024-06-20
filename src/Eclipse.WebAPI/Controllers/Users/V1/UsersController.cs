using Asp.Versioning;

using Eclipse.Application.Contracts.Users;
using Eclipse.WebAPI.Constants;
using Eclipse.WebAPI.Filters.Authorization;

using Microsoft.AspNetCore.Mvc;

namespace Eclipse.WebAPI.Controllers.Users.V1;

[ApiController]
[ApiKeyAuthorize]
[Route("api/users")]
[ApiVersion(ApiVersions.V1.Version, Deprecated = ApiVersions.V1.Deprecated)]
public class UsersController : ControllerBase
{
    private readonly IUserService _service;

    public UsersController(IUserService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        return Ok(await _service.GetAllAsync(cancellationToken));
    }
}
