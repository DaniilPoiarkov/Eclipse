using Eclipse.Application.Contracts.ApiTokens;
using Eclipse.Common.Results;
using Eclipse.WebAPI.Extensions;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace Eclipse.WebAPI.Controllers;

[Authorize]
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

    [HttpDelete("{tokenId:guid}")]
    public async Task<IActionResult> RevokeAsync(Guid tokenId, CancellationToken cancellationToken)
    {
        var result = await _apiTokenService.RevokeAsync(User.GetUserId(), tokenId, cancellationToken);

        return result.Match(NoContent, error => error.ToProblems(_localizer));
    }
}
