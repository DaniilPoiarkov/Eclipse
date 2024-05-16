using Asp.Versioning;

using Eclipse.Application.Contracts.IdentityUsers;
using Eclipse.WebAPI.Constants;
using Eclipse.WebAPI.Filters.Authorization;

using Microsoft.AspNetCore.Mvc;

namespace Eclipse.WebAPI.Controllers.Users.V1;

[ApiController]
[ApiKeyAuthorize]
[Route("api/v{v:apiVersion}/[controller]")]
[ApiVersion(ApiVersions.V1.Version, Deprecated = ApiVersions.V1.Deprecated)]
public class UsersController : ControllerBase
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
}
