using Eclipse.Application.Contracts.Reminders;
using Eclipse.Common.Results;
using Eclipse.Common.Session;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace Eclipse.WebAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/reminders")]
public sealed class RemindersController : ControllerBase
{
    private readonly ICurrentSession _currentSession;

    private readonly IReminderService _reminderService;

    private readonly IStringLocalizer<RemindersController> _stringLocalizer;

    public RemindersController(
        ICurrentSession currentSession,
        IReminderService reminderService,
        IStringLocalizer<RemindersController> stringLocalizer)
    {
        _currentSession = currentSession;
        _reminderService = reminderService;
        _stringLocalizer = stringLocalizer;
    }

    [HttpGet]
    public async Task<IActionResult> GetListAsync(CancellationToken cancellationToken)
    {
        var result = await _reminderService.GetListAsync(_currentSession.UserId, cancellationToken);

        return result.Match(Ok, error => error.ToProblems(_stringLocalizer));
    }

    [HttpGet("{reminderId:guid}", Name = "get-reminder-by-id")]
    public async Task<IActionResult> GetAsync(Guid reminderId, CancellationToken cancellationToken)
    {
        var result = await _reminderService.GetAsync(_currentSession.UserId, reminderId, cancellationToken);

        return result.Match(Ok, error => error.ToProblems(_stringLocalizer));
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] ReminderCreateDto model, CancellationToken cancellationToken)
    {
        var result = await _reminderService.CreateAsync(_currentSession.UserId, model, cancellationToken);

        var createdUrl = result.IsSuccess
            ? Url.Link("get-reminder-by-id", new { reminderId = result.Value.Id })
            : string.Empty;

        return result.Match(value => Created(createdUrl, value), error => error.ToProblems(_stringLocalizer));
    }
}
