using Eclipse.Application.Contracts.Exporting;
using Eclipse.Common.Background;

namespace Eclipse.WebAPI.Background;

public sealed class ImportUsersBackgroundJob : IBackgroundJob<ImportEntitiesBackgroundJobArgs>
{
    private readonly IImportService _importService;

    public ImportUsersBackgroundJob(IImportService importService)
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

        await _importService.AddUsersAsync(stream, cancellationToken);
    }
}
