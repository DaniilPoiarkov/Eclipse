using Telegram.Bot;
using Telegram.Bot.Types;

namespace Eclipse.Core.Results;

public sealed class RedirectResult : ResultBase
{
    public Type PipelineType { get; }

    private readonly IEnumerable<IResult> _results;

    public RedirectResult(Type type, IEnumerable<IResult> results)
    {
        PipelineType = type;
        _results = results;
    }

    public override async Task<Message?> SendAsync(ITelegramBotClient botClient, CancellationToken cancellationToken = default)
    {
        foreach (var result in _results.Cast<ResultBase>())
        {
            result.ChatId = ChatId;
            await result.SendAsync(botClient, cancellationToken);
        }

        return null;
    }
}
