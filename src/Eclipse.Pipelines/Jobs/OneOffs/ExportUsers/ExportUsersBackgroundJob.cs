using Eclipse.Application.Contracts.Exporting;
using Eclipse.Common.Background;

using Telegram.Bot;
using Telegram.Bot.Types;

namespace Eclipse.Pipelines.Jobs.OneOffs.ExportUsers;

internal sealed class ExportUsersBackgroundJob : IBackgroundJob<ExportToUserBackgroundJobArgs>
{
    private readonly IExportService _exportService;

    private readonly ITelegramBotClient _botClient;

    public ExportUsersBackgroundJob(IExportService exportService, ITelegramBotClient botClient)
    {
        _exportService = exportService;
        _botClient = botClient;
    }

    public async Task ExecuteAsync(ExportToUserBackgroundJobArgs args, CancellationToken cancellationToken = default)
    {
        using var stream = await _exportService.GetUsersAsync(cancellationToken);

        await _botClient.SendDocument(
            args.ChatId,
            InputFile.FromStream(stream, "users.xlsx"),
            caption: "users excel table",
            cancellationToken: cancellationToken
        );
    }
}
