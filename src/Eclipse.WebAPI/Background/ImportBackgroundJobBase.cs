using Eclipse.Application.Contracts.Exporting;
using Eclipse.Common.Background;
using Eclipse.Common.Excel;
using Eclipse.Common.Telegram;

using Microsoft.Extensions.Options;

using Telegram.Bot;
using Telegram.Bot.Types;

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

    public async Task ExecuteAsync(ImportEntitiesBackgroundJobArgs args, CancellationToken cancellationToken = default)
    {
        try
        {
            await ImportAsync(args, cancellationToken);
        }
        catch (Exception ex)
        {
            await SendMessageAsync($"Failed to import file: {ex.Message}", cancellationToken);
        }
    }

    protected abstract Task ImportAsync(ImportEntitiesBackgroundJobArgs args, CancellationToken cancellationToken = default);

    protected Task SendMessageAsync(string message, CancellationToken cancellationToken = default)
    {
        return BotClient.SendMessage(Options.Value.Chat, message, cancellationToken: cancellationToken);
    }

    protected async Task SendFailedResultAsync<T>(string caption, string fileName, List<T> failedResult, CancellationToken cancellationToken = default)
    {
        using var stream = ExcelManager.Write(failedResult);

        await BotClient.SendDocument(
            Options.Value.Chat,
            InputFile.FromStream(stream, fileName),
            caption: caption,
            cancellationToken: cancellationToken
        );
    }
}
