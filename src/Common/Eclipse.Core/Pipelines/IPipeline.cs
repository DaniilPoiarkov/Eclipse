using Eclipse.Core.Context;
using Eclipse.Core.Results;

using Telegram.Bot.Types;

namespace Eclipse.Core.Pipelines;

public interface IPipeline
{
    int StagesLeft { get; }

    bool IsFinished { get; }

    void SetUpdate(Update update);

    Task<IResult> RunNext(MessageContext context, CancellationToken cancellationToken = default);

    void SkipStage();
}
