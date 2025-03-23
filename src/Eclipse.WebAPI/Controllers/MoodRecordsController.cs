using Eclipse.Application.Contracts.MoodRecords;
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

    private readonly IStringLocalizer<MoodRecordsController> _localizer;

    public MoodRecordsController(
        IMoodRecordsService moodRecordsService,
        IStringLocalizer<MoodRecordsController> localizer)
    {
        _moodRecordsService = moodRecordsService;
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
}
