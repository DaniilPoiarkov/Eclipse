using Eclipse.Core.Core;

using Telegram.Bot;
using Telegram.Bot.Types;

namespace Eclipse.Core.Results;

internal class MultipleActionsResult : ResultBase
{
    public IList<IResult> Results { get; }

    public MultipleActionsResult(IList<IResult> results)
    {
        Results = results;
    }

    public override async Task<Message?> SendAsync(ITelegramBotClient botClient, CancellationToken cancellationToken = default)
    {
        var messages = new List<Message?>(Results.Count);

        foreach (var result in Results.Cast<ResultBase>())
        {
            result.ChatId = ChatId;
            messages.Add(await result.SendAsync(botClient, cancellationToken));
        }

        if (messages.Count == 0)
        {
            return null;
        }

        return messages
            .Where(m => m is not null)
            .LastOrDefault();
    }
}
