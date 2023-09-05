using Telegram.Bot;
using Telegram.Bot.Types;

namespace Eclipse.Core.Results;

public class TextResult : ResultBase
{
    private readonly string _message;

    public TextResult(string message)
    {
        _message = message;
    }

    public override async Task<Message?> SendAsync(ITelegramBotClient botClient, CancellationToken cancellationToken = default)
    {
        return await botClient.SendTextMessageAsync(ChatId, _message, cancellationToken: cancellationToken);
    }
}
