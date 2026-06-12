using Eclipse.Application.Contracts.Statistics;
using Eclipse.WebAPI.Constants;
using Eclipse.WebAPI.Extensions;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eclipse.WebAPI.Controllers;

[Authorize]
[Authorize(Policy = AuthorizationPolicies.Scopes.UserStatistics)]
[ApiController]
[Route("api/user-statistics")]
public sealed class UserStatisticsController : ControllerBase
{
    private readonly IUserStatisticsService _userStatisticsService;

    public UserStatisticsController(IUserStatisticsService userStatisticsService)
    {
        _userStatisticsService = userStatisticsService;
    }

    [HttpGet]
    public async Task<IActionResult> GetStatisticsAsync(CancellationToken cancellationToken)
    {
        return Ok(await _userStatisticsService.GetByUserIdAsync(User.GetUserId(), cancellationToken));
    }
}
