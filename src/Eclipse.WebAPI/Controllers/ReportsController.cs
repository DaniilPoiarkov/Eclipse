using Eclipse.Application.Contracts.Reports;
using Eclipse.Common.Clock;
using Eclipse.Common.ContentTypes;
using Eclipse.Common.Session;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eclipse.WebAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/reports")]
public sealed class ReportsController : ControllerBase
{
    private readonly ICurrentSession _currentSession;

    private readonly IReportsService _reportsService;

    private readonly ITimeProvider _timeProvider;

    public ReportsController(ICurrentSession currentSession, IReportsService reportsService, ITimeProvider timeProvider)
    {
        _currentSession = currentSession;
        _reportsService = reportsService;
        _timeProvider = timeProvider;
    }

    [HttpGet("mood")]
    public async Task<IActionResult> GetMoodReportAsync(CancellationToken cancellationToken)
    {
        if (!_currentSession.UserId.HasValue)
        {
            return Unauthorized();
        }

        var stream = await _reportsService.GetMoodReportAsync(_currentSession.UserId.Value, new MoodReportOptions
        {
            From = _timeProvider.Now.PreviousDayOfWeek(
                _timeProvider.Now.DayOfWeek
            ),
            To = _timeProvider.Now
        }, cancellationToken);

        return File(stream, MimeContentTypes.ImagePng, "mood-report.png");
    }
}
