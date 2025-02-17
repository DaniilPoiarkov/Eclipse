using Eclipse.Application.Contracts.Exporting;
using Eclipse.Common.ContentTypes;
using Eclipse.WebAPI.Constants;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Eclipse.WebAPI.Controllers;

[ApiController]
[Route("api/export")]
[Authorize(Policy = AuthorizationPolicies.Admin)]
[EnableRateLimiting(RateLimiterPolicies.IpAddress)]
public sealed class ExportController : ControllerBase
{
    private readonly IExportService _exportService;

    public ExportController(IExportService exportService)
    {
        _exportService = exportService;
    }

    [HttpGet("users")]
    public async Task<IActionResult> Users(CancellationToken cancellationToken)
    {
        var stream = await _exportService.GetUsersAsync(cancellationToken);

        return File(stream, MimeContentTypes.ApplicationVndOpenxmlformattsOfficedocumentSpreadsheetmlSheet, "users.xlsx");
    }

    [HttpGet("todo-items")]
    public async Task<IActionResult> TodoItems(CancellationToken cancellationToken)
    {
        var stream = await _exportService.GetTodoItemsAsync(cancellationToken);

        return File(stream, MimeContentTypes.ApplicationVndOpenxmlformattsOfficedocumentSpreadsheetmlSheet, "todo-items.xlsx");
    }

    [HttpGet("reminders")]
    public async Task<IActionResult> Reminders(CancellationToken cancellationToken)
    {
        var stream = await _exportService.GetRemindersAsync(cancellationToken);

        return File(stream, MimeContentTypes.ApplicationVndOpenxmlformattsOfficedocumentSpreadsheetmlSheet, "reminders.xlsx");
    }
}
