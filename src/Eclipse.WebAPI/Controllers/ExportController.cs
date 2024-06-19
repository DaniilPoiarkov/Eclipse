using Eclipse.Application.Contracts.Exporting;
using Eclipse.Common.ContentTypes;
using Eclipse.WebAPI.Filters.Authorization;

using Microsoft.AspNetCore.Mvc;

namespace Eclipse.WebAPI.Controllers;

[ApiController]
[ApiKeyAuthorize]
[Route("api/[controller]/[action]")]
public sealed class ExportController : ControllerBase
{
    private readonly IExportService _exportService;

    public ExportController(IExportService exportService)
    {
        _exportService = exportService;
    }

    [HttpGet]
    public async Task<IActionResult> Users(CancellationToken cancellationToken)
    {
        var stream = await _exportService.GetUsersAsync(cancellationToken);

        return File(stream, MimeContentTypes.ApplicationVndOpenxmlformattsOfficedocumentSpreadsheetmlSheet, "users.xlsx");
    }

    [HttpGet]
    public async Task<IActionResult> TodoItems(CancellationToken cancellationToken)
    {
        var stream = await _exportService.GetTodoItemsAsync(cancellationToken);

        return File(stream, MimeContentTypes.ApplicationVndOpenxmlformattsOfficedocumentSpreadsheetmlSheet, "todo-items.xlsx");
    }

    [HttpGet]
    public async Task<IActionResult> Reminders(CancellationToken cancellationToken)
    {
        var stream = await _exportService.GetRemindersAsync(cancellationToken);

        return File(stream, MimeContentTypes.ApplicationVndOpenxmlformattsOfficedocumentSpreadsheetmlSheet, "reminders.xlsx");
    }
}
