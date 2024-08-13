using Eclipse.Application.Contracts.Exporting;
using Eclipse.Common.Excel;
using Eclipse.Common.Telegram;

using Microsoft.Extensions.Options;

using Telegram.Bot;

namespace Eclipse.WebAPI.Background;

public sealed class ImportRemindersBackgroundJob : ImportBackgroundJobBase
{
    public ImportRemindersBackgroundJob(
        IImportService importService,
        IExcelManager excelManager,
        ITelegramBotClient botClient,
        IOptions<TelegramOptions> options)
        : base(importService, excelManager, botClient, options) { }

    protected async override Task ImportAsync(ImportEntitiesBackgroundJobArgs args, CancellationToken cancellationToken = default)
    {
        if (args.BytesAsBase64.IsNullOrEmpty())
        {
            return;
        }

        var bytes = Convert.FromBase64String(args.BytesAsBase64);

        using var stream = new MemoryStream(bytes);

        var result = await ImportService.AddRemindersAsync(stream, cancellationToken);

        if (result.IsSuccess)
        {
            await SendMessageAsync("All reminders imported successfully.", cancellationToken);
            return;
        }

        await SendFailedResultAsync(
            "Failed to import following reminders",
            "failed-to-import-reminders.xlsx",
            result.FailedRows,
            cancellationToken
        );
    }
}
