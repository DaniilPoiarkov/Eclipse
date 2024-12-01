using Telegram.Bot;
using Telegram.Bot.Types;

namespace Eclipse.Core.Results;

public sealed class PhotoResult : ResultBase
{
    private readonly MemoryStream _stream;

    private readonly string _fileName;

    private readonly string? _caption;

    public PhotoResult(MemoryStream stream, string fileName, string? caption = null)
    {
        _stream = stream;
        _fileName = fileName;
        _caption = caption;
    }

    public override async Task<Message?> SendAsync(ITelegramBotClient botClient, CancellationToken cancellationToken = default)
    {
        using var _ = _stream;

        return await botClient.SendPhoto(
            ChatId,
            InputFile.FromStream(_stream, _fileName),
            caption: _caption,
            cancellationToken: cancellationToken
        );
    }
}
