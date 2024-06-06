using Eclipse.Application.Contracts.Exporting;
using Eclipse.Common.ContentTypes;
using Eclipse.WebAPI.Filters.Authorization;

using Microsoft.AspNetCore.Mvc;

namespace Eclipse.WebAPI.Controllers;

[ApiController]
[ApiKeyAuthorize]
[Route("api/[controller]/[action]")]
public class ExportController : ControllerBase
{
    private readonly IUserExporter _userExporter;

    public ExportController(IUserExporter userExporter)
    {
        _userExporter = userExporter;
    }

    [HttpGet]
    public async Task<IActionResult> Users(CancellationToken cancellationToken)
    {
        var stream = await _userExporter.ExportAllAsync(cancellationToken);

        return File(stream, MimeContentTypes.ApplicationVndOpenxmlformattsOfficedocumentSpreadsheetmlSheet, "users.xlsx");
    }
}
