using Eclipse.Application.Contracts.Statistics;
using Eclipse.Common.Session;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eclipse.WebAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/user-statistics")]
public sealed class UserStatisticsController : ControllerBase
{
    private readonly ICurrentSession _currentSession;

    private readonly IUserStatisticsService _userStatisticsService;

    public UserStatisticsController(
        ICurrentSession currentSession,
        IUserStatisticsService userStatisticsService)
    {
        _currentSession = currentSession;
        _userStatisticsService = userStatisticsService;
    }

    [HttpGet]
    public async Task<IActionResult> GetStatisticsAsync(CancellationToken cancellationToken)
    {
        return Ok(await _userStatisticsService.GetByUserIdAsync(_currentSession.UserId, cancellationToken));
    }
}
