using Eclipse.Application.Contracts.Exporting;
using Eclipse.Common.Background;

using Telegram.Bot;
using Telegram.Bot.Types;

namespace Eclipse.Pipelines.Pipelines.AdminMenu.Export.Jobs;

internal sealed class ExportRemindersBackgroundJob : IBackgroundJob<ExportToUserBackgroundJobArgs>
{
    private readonly IExportService _exportService;

    private readonly ITelegramBotClient _botClient;

    public ExportRemindersBackgroundJob(IExportService exportService, ITelegramBotClient botClient)
    {
        _exportService = exportService;
        _botClient = botClient;
    }

    public async Task ExecuteAsync(ExportToUserBackgroundJobArgs args, CancellationToken cancellationToken = default)
    {
        using var stream = await _exportService.GetRemindersAsync(cancellationToken);

        await _botClient.SendDocument(
            args.ChatId,
            InputFile.FromStream(stream, "reminders.xlsx"),
            caption: "reminders excel table",
            cancellationToken: cancellationToken
        );
    }
}
