using Eclipse.Application.Contracts.Exporting;
using Eclipse.Common.Background;

using Telegram.Bot;
using Telegram.Bot.Types;

namespace Eclipse.Pipelines.Jobs.OneOffs.ExportTodoItems;

internal sealed class ExportTodoItemsBackgroundJob : IBackgroundJob<ExportToUserBackgroundJobArgs>
{
    private readonly ITelegramBotClient _botClient;

    private readonly IExportService _exportService;

    public ExportTodoItemsBackgroundJob(ITelegramBotClient botClient, IExportService exportService)
    {
        _botClient = botClient;
        _exportService = exportService;
    }

    public async Task ExecuteAsync(ExportToUserBackgroundJobArgs args, CancellationToken cancellationToken = default)
    {
        using var stream = await _exportService.GetTodoItemsAsync(cancellationToken);

        await _botClient.SendDocument(
            args.ChatId,
            InputFile.FromStream(stream, "todo-items.xlsx"),
            caption: "todo items excel table",
            cancellationToken: cancellationToken
        );
    }
}
