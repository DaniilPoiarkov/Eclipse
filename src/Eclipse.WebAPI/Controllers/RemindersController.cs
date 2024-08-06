using Eclipse.Application.Contracts.Reminders;
using Eclipse.Application.Contracts.Users;
using Eclipse.Common.Results;
using Eclipse.Common.Session;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eclipse.WebAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/reminders")]
public sealed class RemindersController : ControllerBase
{
    private readonly ICurrentSession _currentSession;

    private readonly IReminderService _reminderService;

    public RemindersController(ICurrentSession currentSession, IReminderService reminderService)
    {
        _currentSession = currentSession;
        _reminderService = reminderService;
    }

    [HttpGet]
    public async Task<IActionResult> GetListAsync(CancellationToken cancellationToken)
    {
        if (!_currentSession.UserId.HasValue)
        {
            return Unauthorized();
        }

        var result = await _reminderService.GetListAsync(_currentSession.UserId.Value, cancellationToken);

        return result.Match(() => Ok(result.Value), result.ToProblems);
    }

    [HttpGet("{reminderId:guid}", Name = "get-reminder-by-id")]
    public async Task<IActionResult> GetAsync(Guid reminderId, CancellationToken cancellationToken)
    {
        if (!_currentSession.UserId.HasValue)
        {
            return Unauthorized();
        }

        var result = await _reminderService.GetAsync(_currentSession.UserId.Value, reminderId, cancellationToken);

        return result.Match(() => Ok(result.Value), result.ToProblems);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] ReminderCreateDto model, CancellationToken cancellationToken)
    {
        if (!_currentSession.UserId.HasValue)
        {
            return Unauthorized();
        }

        var result = await _reminderService.CreateAsync(_currentSession.UserId.Value, model, cancellationToken);

        var createdUrl = result.IsSuccess
            ? Url.Link("get-reminder-by-id", new { reminderId = result.Value.Id })
            : string.Empty;

        return result.Match(() => Created(createdUrl, result.Value), result.ToProblems);
    }
}
