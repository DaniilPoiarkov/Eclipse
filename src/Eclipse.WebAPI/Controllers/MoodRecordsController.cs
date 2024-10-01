using Eclipse.Application.Contracts.MoodRecords;
using Eclipse.Common.Results;
using Eclipse.Common.Session;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace Eclipse.WebAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/mood-records")]
public class MoodRecordsController : ControllerBase
{
    private readonly IMoodRecordsService _moodRecordsService;

    private readonly ICurrentSession _currentSession;

    private readonly IStringLocalizer<MoodRecordsController> _localizer;

    public MoodRecordsController(
        IMoodRecordsService moodRecordsService,
        ICurrentSession currentSession,
        IStringLocalizer<MoodRecordsController> localizer)
    {
        _moodRecordsService = moodRecordsService;
        _currentSession = currentSession;
        _localizer = localizer;
    }

    [HttpGet]
    public async Task<IActionResult> GetList()
    {
        if (!_currentSession.UserId.HasValue)
        {
            return Unauthorized();
        }

        return Ok(await _moodRecordsService.GetListAsync(_currentSession.UserId.Value));
    }

    [HttpGet("{moodRecordId:guid}", Name = "get-mood-record-by-id")]
    public async Task<IActionResult> GetByIdAsync(Guid moodRecordId, CancellationToken cancellationToken)
    {
        if (!_currentSession.UserId.HasValue)
        {
            return Unauthorized();
        }

        var result = await _moodRecordsService.GetByIdAsync(_currentSession.UserId.Value, moodRecordId, cancellationToken);

        return result.Match(() => Ok(result.Value), () => result.ToProblems(_localizer));
    }

    [HttpPost("add")]
    public async Task<IActionResult> CreateAsync([FromBody] CreateMoodRecordDto model, CancellationToken cancellationToken)
    {
        if (!_currentSession.UserId.HasValue)
        {
            return Unauthorized();
        }

        var result = await _moodRecordsService.CreateAsync(_currentSession.UserId.Value, model, cancellationToken);

        var createdUrl = result.IsSuccess
            ? Url.Link("get-mood-record-by-id", new { moodRecordId = result.Value.Id })
            : string.Empty;

        return result.Match(() => Created(createdUrl, result.Value), () => result.ToProblems(_localizer));
    }
}
