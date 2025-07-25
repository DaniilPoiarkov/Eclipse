﻿using Eclipse.Application.Contracts.Suggestions;
using Eclipse.WebAPI.Constants;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eclipse.WebAPI.Controllers;

[ApiController]
[Route("api/suggestions")]
[Authorize(Policy = AuthorizationPolicies.Admin)]
public sealed class SuggestionsController : ControllerBase
{
    private readonly ISuggestionsService _suggestionsService;

    public SuggestionsController(ISuggestionsService suggestionsService)
    {
        _suggestionsService = suggestionsService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        return Ok(await _suggestionsService.GetWithUserInfo(cancellationToken));
    }
}
