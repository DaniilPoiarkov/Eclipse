using Telegram.Bot;
using Telegram.Bot.Types;

namespace Eclipse.Core.Results;

public sealed class PhotoResult : ResultBase
{
    private readonly InputFile _file;

    public PhotoResult(MemoryStream stream, string fileName)
    {
        _file = InputFile.FromStream(stream, fileName);
    }

    public override async Task<Message?> SendAsync(ITelegramBotClient botClient, CancellationToken cancellationToken = default)
    {
        return await botClient.SendPhotoAsync(ChatId, _file, cancellationToken: cancellationToken);
    }
}
