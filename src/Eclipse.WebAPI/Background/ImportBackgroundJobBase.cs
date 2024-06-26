using Eclipse.Application.Contracts.Exporting;
using Eclipse.Common.Background;
using Eclipse.Common.Excel;
using Eclipse.Common.Telegram;

using Microsoft.Extensions.Options;

using System.IO;

using Telegram.Bot;
using Telegram.Bot.Types;

using static Microsoft.IO.RecyclableMemoryStreamManager;

namespace Eclipse.WebAPI.Background;

public abstract class ImportBackgroundJobBase : IBackgroundJob<ImportEntitiesBackgroundJobArgs>
{
    protected readonly IImportService ImportService;

    protected readonly IExcelManager ExcelManager;

    protected readonly ITelegramBotClient BotClient;

    protected readonly IOptions<TelegramOptions> Options;

    protected ImportBackgroundJobBase(
        IImportService importService,
        IExcelManager excelManager,
        ITelegramBotClient botClient,
        IOptions<TelegramOptions> options)
    {
        ImportService = importService;
        ExcelManager = excelManager;
        BotClient = botClient;
        Options = options;
    }

    public abstract Task ExecureAsync(ImportEntitiesBackgroundJobArgs args, CancellationToken cancellationToken = default);

    protected Task SendSuccessResult(string message, CancellationToken cancellationToken = default)
    {
        return BotClient.SendTextMessageAsync(Options.Value.Chat, message, cancellationToken: cancellationToken);
    }

    protected async Task SendFailedResult<T>(string caption, string fileName, List<T> failedResult, CancellationToken cancellationToken = default)
    {
        using var stream = ExcelManager.Write(failedResult);

        await BotClient.SendDocumentAsync(
            Options.Value.Chat,
            InputFile.FromStream(stream, fileName),
            caption: caption,
            cancellationToken: cancellationToken
        );
    }
}
