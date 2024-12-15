using Eclipse.Core.Core;

using Telegram.Bot;
using Telegram.Bot.Types;

namespace Eclipse.Core.Results;

public sealed class RedirectResult : ResultBase
{
    internal Type PipelineType { get; }

    private readonly IEnumerable<IResult> _results;

    public RedirectResult(Type type, IEnumerable<IResult> results)
    {
        PipelineType = type;
        _results = results;
    }

    public override async Task<Message?> SendAsync(ITelegramBotClient botClient, CancellationToken cancellationToken = default)
    {
        foreach (var result in _results)
        {
            await result.SendAsync(botClient, cancellationToken);
        }

        return null;
    }
}
