using Eclipse.Application.Contracts.Exporting;
using Eclipse.Common.Background;

using Telegram.Bot;
using Telegram.Bot.Types;

namespace Eclipse.Pipelines.Jobs.OneOffs.ExportReminders;

internal sealed class ExportRemindersBackgroundJob : IBackgroundJob<ExportToUserBackgroundJobArgs>
{
    private readonly IExportService _exportService;

    private readonly ITelegramBotClient _botClient;

    public ExportRemindersBackgroundJob(IExportService exportService, ITelegramBotClient botClient)
    {
        _exportService = exportService;
        _botClient = botClient;
    }

    public async Task ExecureAsync(ExportToUserBackgroundJobArgs args, CancellationToken cancellationToken = default)
    {
        using var stream = await _exportService.GetRemindersAsync(cancellationToken);

        await _botClient.SendDocumentAsync(
            args.ChatId,
            InputFile.FromStream(stream, "reminders.xlsx"),
            caption: "reminders excel table",
            cancellationToken: cancellationToken
        );
    }
}
