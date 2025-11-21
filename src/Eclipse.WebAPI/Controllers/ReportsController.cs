using Eclipse.Application.Contracts.Reports;
using Eclipse.Common.Clock;
using Eclipse.Common.ContentTypes;
using Eclipse.WebAPI.Extensions;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eclipse.WebAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/reports")]
public sealed class ReportsController : ControllerBase
{
    private readonly IReportsService _reportsService;

    private readonly ITimeProvider _timeProvider;

    public ReportsController(IReportsService reportsService, ITimeProvider timeProvider)
    {
        _reportsService = reportsService;
        _timeProvider = timeProvider;
    }

    [HttpGet("mood")]
    public async Task<IActionResult> GetMoodReportAsync(CancellationToken cancellationToken)
    {
        var stream = await _reportsService.GetMoodReportAsync(User.GetUserId(), new MoodReportOptions
        {
            From = _timeProvider.Now.PreviousDayOfWeek(
                _timeProvider.Now.DayOfWeek
            ).WithTime(0, 0),
            To = _timeProvider.Now.WithTime(23, 59)
        }, cancellationToken);

        return File(stream, MimeContentTypes.ImagePng, "mood-report.png");
    }
}
