using Eclipse.Application.Contracts.Exporting;
using Eclipse.Common.Excel;
using Eclipse.Common.Telegram;

using Microsoft.Extensions.Options;

using Telegram.Bot;

namespace Eclipse.WebAPI.Background;

public sealed class ImportUsersBackgroundJob : ImportBackgroundJobBase
{
    public ImportUsersBackgroundJob(
        IImportService importService,
        IExcelManager excelManager,
        ITelegramBotClient botClient,
        IOptions<TelegramOptions> options)
        : base(importService, excelManager, botClient, options) { }

    public async override Task ExecureAsync(ImportEntitiesBackgroundJobArgs args, CancellationToken cancellationToken = default)
    {
        if (args.BytesAsBase64.IsNullOrEmpty())
        {
            return;
        }

        var bytes = Convert.FromBase64String(args.BytesAsBase64);

        using var stream = new MemoryStream(bytes);

        var result = await ImportService.AddUsersAsync(stream, cancellationToken);

        if (result.IsSuccess)
        {
            await SendSuccessResult("All users imported successfully.", cancellationToken);
            return;
        }

        await SendFailedResult(
            "Failed to import following users",
            "failed-to-import-users.xlsx",
            result.FailedRows,
            cancellationToken
        );
    }
}
