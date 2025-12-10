using Eclipse.Application.Contracts.MoodRecords;
using Eclipse.Common.Clock;
using Eclipse.Common.ContentTypes;
using Eclipse.Common.Results;
using Eclipse.WebAPI.Extensions;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace Eclipse.WebAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/mood-records")]
public sealed class MoodRecordsController : ControllerBase
{
    private readonly IMoodRecordsService _moodRecordsService;

    private readonly IMoodReportService _reportsService;

    private readonly ITimeProvider _timeProvider;

    private readonly IStringLocalizer<MoodRecordsController> _localizer;

    public MoodRecordsController(
        IMoodRecordsService moodRecordsService,
        IMoodReportService reportsService,
        ITimeProvider timeProvider,
        IStringLocalizer<MoodRecordsController> localizer)
    {
        _moodRecordsService = moodRecordsService;
        _reportsService = reportsService;
        _timeProvider = timeProvider;
        _localizer = localizer;
    }

    [HttpGet]
    public async Task<IActionResult> GetList()
    {
        return Ok(await _moodRecordsService.GetListAsync(User.GetUserId()));
    }

    [HttpGet("{moodRecordId:guid}", Name = "get-mood-record-by-id")]
    public async Task<IActionResult> GetByIdAsync(Guid moodRecordId, CancellationToken cancellationToken)
    {
        var result = await _moodRecordsService.GetByIdAsync(User.GetUserId(), moodRecordId, cancellationToken);

        return result.Match(Ok, error => error.ToProblems(_localizer));
    }

    [HttpPost("add")]
    public async Task<IActionResult> CreateAsync([FromBody] CreateMoodRecordDto model, CancellationToken cancellationToken)
    {
        var result = await _moodRecordsService.CreateOrUpdateAsync(User.GetUserId(), model, cancellationToken);

        var createdUrl = result.IsSuccess
            ? Url.Link("get-mood-record-by-id", new { moodRecordId = result.Value.Id })
            : string.Empty;

        return result.Match(value => Created(createdUrl, value), error => error.ToProblems(_localizer));
    }

    [HttpGet("report")]
    public async Task<IActionResult> GetMoodReportAsync(CancellationToken cancellationToken)
    {
        var stream = await _reportsService.GetAsync(User.GetUserId(), new MoodReportOptions
        {
            From = _timeProvider.Now.PreviousDayOfWeek(
                _timeProvider.Now.DayOfWeek
            ).WithTime(0, 0),
            To = _timeProvider.Now.WithTime(23, 59)
        }, cancellationToken);

        return File(stream, MimeContentTypes.ImagePng, "mood-report.png");
    }
}
