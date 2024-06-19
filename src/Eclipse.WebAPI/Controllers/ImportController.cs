using Eclipse.Common.Background;
using Eclipse.WebAPI.Background;
using Eclipse.WebAPI.Extensions;
using Eclipse.WebAPI.Filters.Authorization;

using Microsoft.AspNetCore.Mvc;

namespace Eclipse.WebAPI.Controllers;

[ApiController]
[ApiKeyAuthorize]
[Route("api/[controller]/[action]")]
public sealed class ImportController : ControllerBase
{
    private readonly IBackgroundJobManager _jobManager;

    public ImportController(IBackgroundJobManager jobManager)
    {
        _jobManager = jobManager;
    }

    [HttpPost]
    public async Task<IActionResult> Users(IFormFile file, CancellationToken cancellationToken)
    {
        await EnqueueJob<ImportUsersBackgroundJob>(file, cancellationToken);

        return Accepted();
    }

    [HttpPost]
    public async Task<IActionResult> TodoItems(IFormFile file, CancellationToken cancellationToken)
    {
        await EnqueueJob<ImportTodoItemsBackgroundJob>(file, cancellationToken);

        return Accepted();
    }

    [HttpPost]
    public async Task<IActionResult> Reminders(IFormFile file, CancellationToken cancellationToken)
    {
        await EnqueueJob<ImportRemindersBackgroundJob>(file, cancellationToken);

        return Accepted();
    }

    private async Task EnqueueJob<TBackgroundJob>(IFormFile file, CancellationToken cancellationToken)
        where TBackgroundJob : IBackgroundJob<ImportEntitiesBackgroundJobArgs>
    {
        var bytes = await file.ReadAllBytesAsync(cancellationToken);

        await _jobManager.EnqueueAsync<TBackgroundJob, ImportEntitiesBackgroundJobArgs>(
            new ImportEntitiesBackgroundJobArgs { BytesAsBase64 = Convert.ToBase64String(bytes) },
            cancellationToken
        );
    }
}
