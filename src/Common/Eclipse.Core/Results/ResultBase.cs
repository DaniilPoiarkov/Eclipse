using Telegram.Bot;
using Telegram.Bot.Types;

namespace Eclipse.Core.Results;

public abstract class ResultBase : IResult
{
    public long ChatId { get; set; }

    public abstract Task<Message?> SendAsync(ITelegramBotClient botClient, CancellationToken cancellationToken = default);
}
