using Eclipse.Core.Core;

using Telegram.Bot;
using Telegram.Bot.Types;

namespace Eclipse.Core.Results;

internal class MultipleActionsResult : ResultBase
{
    private readonly IList<IResult> _results;

    public MultipleActionsResult(IList<IResult> results)
    {
        _results = results;
    }

    public override async Task<Message?> SendAsync(ITelegramBotClient botClient, CancellationToken cancellationToken = default)
    {
        var messages = new List<Message?>(_results.Count);

        foreach (var result in _results.Cast<ResultBase>())
        {
            result.ChatId = ChatId;
            messages.Add(await result.SendAsync(botClient, cancellationToken));
        }

        if (messages is null)
        {
            return null;
        }

        return messages
            .Where(m => m is not null)
            .LastOrDefault();
    }
}
