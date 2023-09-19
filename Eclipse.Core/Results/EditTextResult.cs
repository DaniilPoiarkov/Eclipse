using Telegram.Bot;
using Telegram.Bot.Types;

namespace Eclipse.Core.Results;

public class EditTextResult : ResultBase
{
    private readonly int _messageId;

    private readonly string _text;

    public EditTextResult(int messageId, string text)
    {
        _messageId = messageId;
        _text = text;
    }

    public override async Task<Message?> SendAsync(ITelegramBotClient botClient, CancellationToken cancellationToken = default)
    {
        return await botClient.EditMessageTextAsync(ChatId, _messageId, _text, cancellationToken: cancellationToken);
    }
}
