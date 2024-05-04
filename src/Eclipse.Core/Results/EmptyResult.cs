using Telegram.Bot;
using Telegram.Bot.Types;

namespace Eclipse.Core.Results;

public sealed class EmptyResult : ResultBase
{
    public override Task<Message?> SendAsync(ITelegramBotClient botClient, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<Message?>(null);
    }
}
