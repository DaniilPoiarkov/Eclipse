using Eclipse.Application.Contracts.Exporting;
using Eclipse.Common.Background;

namespace Eclipse.WebAPI.Background;

public sealed class ImportRemindersBackgroundJob : IBackgroundJob<ImportEntitiesBackgroundJobArgs>
{
    private readonly IImportService _importService;

    public ImportRemindersBackgroundJob(IImportService importService)
    {
        _importService = importService;
    }

    public async Task ExecureAsync(ImportEntitiesBackgroundJobArgs args, CancellationToken cancellationToken = default)
    {
        if (args.BytesAsBase64.IsNullOrEmpty())
        {
            return;
        }

        var bytes = Convert.FromBase64String(args.BytesAsBase64);

        using var stream = new MemoryStream(bytes);

        await _importService.AddRemindersAsync(stream, cancellationToken);
    }
}
