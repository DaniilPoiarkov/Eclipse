using Telegram.Bot;
using Telegram.Bot.Types;

namespace Eclipse.Core.Results;

public sealed class MultipleActionsResult : ResultBase
{
    public IList<IResult> Results { get; }

    public MultipleActionsResult(IList<IResult> results)
    {
        Results = results;
    }

    public override async Task<Message?> SendAsync(ITelegramBotClient botClient, CancellationToken cancellationToken = default)
    {
        var messages = new List<Message>(Results.Count);

        foreach (var result in Results.Cast<ResultBase>())
        {
            result.ChatId = ChatId;
            var message = await result.SendAsync(botClient, cancellationToken);

            if (message is not null)
            {
                messages.Add(message);
            }
        }

        return messages.LastOrDefault();
    }
}
