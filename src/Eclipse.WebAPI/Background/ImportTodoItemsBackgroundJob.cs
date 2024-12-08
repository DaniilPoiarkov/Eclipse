using Eclipse.Application;
using Eclipse.Application.Contracts.Exporting;
using Eclipse.Common.Excel;

using Microsoft.Extensions.Options;

using Telegram.Bot;

namespace Eclipse.WebAPI.Background;

public sealed class ImportTodoItemsBackgroundJob : ImportBackgroundJobBase
{
    public ImportTodoItemsBackgroundJob(
        IImportService importService,
        IExcelManager excelManager,
        ITelegramBotClient botClient,
        IOptions<ApplicationOptions> options)
        : base(importService, excelManager, botClient, options) { }

    protected async override Task ImportAsync(ImportEntitiesBackgroundJobArgs args, CancellationToken cancellationToken = default)
    {
        if (args.BytesAsBase64.IsNullOrEmpty())
        {
            return;
        }

        var bytes = Convert.FromBase64String(args.BytesAsBase64);

        using var stream = new MemoryStream(bytes);

        var result = await ImportService.AddTodoItemsAsync(stream, cancellationToken);

        if (result.IsSuccess)
        {
            await SendMessageAsync("All todo items imported successfully.", cancellationToken);
            return;
        }

        await SendFailedResultAsync(
            "Failed to import following todo items",
            "failed-to-import-todo-items.xlsx",
            result.FailedRows,
            cancellationToken
        );
    }
}
