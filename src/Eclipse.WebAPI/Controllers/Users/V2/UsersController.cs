﻿using Asp.Versioning;

using Eclipse.Application.Contracts.Users;
using Eclipse.Common.Linq;
using Eclipse.WebAPI.Constants;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eclipse.WebAPI.Controllers.Users.V2;

[ApiController]
[Authorize(Policy = AuthorizationPolicies.Admin)]
[Route("api/v{version:apiVersion}/users")]
[ApiVersion(ApiVersions.V2.Version, Deprecated = ApiVersions.V2.Deprecated)]
public sealed class UsersController : ControllerBase
{
    private readonly IUserService _service;

    public UsersController(IUserService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetList([FromQuery] PaginationRequest<GetUsersRequest> request, CancellationToken cancellationToken)
    {
        return Ok(await _service.GetListAsync(request, cancellationToken));
    }
}
