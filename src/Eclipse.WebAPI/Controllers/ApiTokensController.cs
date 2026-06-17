using Eclipse.Application.Contracts.ApiTokens;
using Eclipse.Common.Results;
using Eclipse.Domain.Shared.ApiTokens;
using Eclipse.WebAPI.Constants;
using Eclipse.WebAPI.Extensions;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

using System.Security.Claims;

namespace Eclipse.WebAPI.Controllers;

[Authorize]
[Authorize(Policy = AuthorizationPolicies.Scopes.ApiTokens)]
[ApiController]
[Route("api/api-tokens")]
public sealed class ApiTokensController : ControllerBase
{
    private readonly IApiTokenService _apiTokenService;

    private readonly IStringLocalizer<ApiTokensController> _localizer;

    public ApiTokensController(IApiTokenService apiTokenService, IStringLocalizer<ApiTokensController> localizer)
    {
        _apiTokenService = apiTokenService;
        _localizer = localizer;
    }

    [HttpGet("scopes")]
    public IActionResult GetScopes()
    {
        var role = User.FindFirstValue(ClaimTypes.Role) ?? string.Empty;
        return Ok(ApiTokenScopeHelper.GetAvailableScopeInfos(role));
    }

    [HttpGet]
    public async Task<IActionResult> GetListAsync(CancellationToken cancellationToken)
    {
        var result = await _apiTokenService.GetListAsync(User.GetUserId(), cancellationToken);

        return result.Match(Ok, error => error.ToProblems(_localizer));
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] CreateApiTokenDto dto, CancellationToken cancellationToken)
    {
        var result = await _apiTokenService.CreateAsync(User.GetUserId(), dto, cancellationToken);

        return result.Match(Ok, error => error.ToProblems(_localizer));
    }

    [HttpDelete("{nameOrId}")]
    public async Task<IActionResult> RevokeAsync(string nameOrId, CancellationToken cancellationToken)
    {
        var result = Guid.TryParse(nameOrId, out Guid tokenId)
            ? await _apiTokenService.RevokeAsync(User.GetUserId(), tokenId, cancellationToken)
            : await _apiTokenService.RevokeByNameAsync(User.GetUserId(), nameOrId, cancellationToken);

        return result.Match(NoContent, error => error.ToProblems(_localizer));
    }
}
