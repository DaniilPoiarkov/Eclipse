using Eclipse.Application.Contracts.Exporting;
using Eclipse.Common.Background;

using Telegram.Bot;
using Telegram.Bot.Types;

namespace Eclipse.Pipelines.Jobs.OneOffs.ExportUsers;

internal sealed class ExportUsersBackgroundJob : IBackgroundJob<ExportUsersBackgroundJobArgs>
{
    private readonly IUserExporter _userExporter;

    private readonly ITelegramBotClient _botClient;

    public ExportUsersBackgroundJob(IUserExporter userExporter, ITelegramBotClient botClient)
    {
        _userExporter = userExporter;
        _botClient = botClient;
    }

    public async Task ExecureAsync(ExportUsersBackgroundJobArgs args, CancellationToken cancellationToken = default)
    {
        var stream = await _userExporter.ExportAllAsync(cancellationToken);

        await _botClient.SendDocumentAsync(
            args.ChatId,
            InputFile.FromStream(stream, "users.xlsx"),
            caption: "users excel table",
            cancellationToken: cancellationToken
        );
    }
}
