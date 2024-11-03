using Eclipse.Application.Contracts.Statistics;
using Eclipse.Common.Results;
using Eclipse.Common.Session;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace Eclipse.WebAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/user-statistics")]
public sealed class UserStatisticsController : ControllerBase
{
    private readonly ICurrentSession _currentSession;

    private readonly IUserStatisticsService _userStatisticsService;

    private readonly IStringLocalizer<UserStatisticsController> _localizer;

    public UserStatisticsController(
        ICurrentSession currentSession,
        IUserStatisticsService userStatisticsService,
        IStringLocalizer<UserStatisticsController> localizer)
    {
        _currentSession = currentSession;
        _userStatisticsService = userStatisticsService;
        _localizer = localizer;
    }

    [HttpGet]
    public async Task<IActionResult> GetStatisticsAsync(CancellationToken cancellationToken)
    {
        var result = await _userStatisticsService.GetByUserIdAsync(_currentSession.UserId, cancellationToken);

        return result.Match(Ok, error => error.ToProblems(_localizer));
    }
}
