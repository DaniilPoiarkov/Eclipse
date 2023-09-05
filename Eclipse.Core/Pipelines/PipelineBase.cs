using Eclipse.Core.Core;
using Eclipse.Core.Results;

namespace Eclipse.Core.Pipelines;

public abstract class PipelineBase : Pipeline
{
    public bool IsFinished => _stages.Count == 0;

    private readonly Queue<IStage> _stages = new();

    public PipelineBase()
    {
        Initialize();
    }

    public async Task<IResult> RunNext(MessageContext context, CancellationToken cancellationToken = default)
    {
        if (IsFinished)
        {
            return new EmptyResult()
            {
                ChatId = context.ChatId
            };
        }

        var next = _stages.Dequeue();

        var result = await next.RunAsync(context, cancellationToken) as ResultBase
            ?? new EmptyResult();

        result.ChatId = context.ChatId;

        return result;
    }

    /// <summary>
    /// Use this method to register stages using <a cref="RegisterStage"></a>
    /// </summary>
    protected abstract void Initialize();

    protected void RegisterStage(IStage stage) => _stages.Enqueue(stage);
    protected void RegisterStage(Func<MessageContext, CancellationToken, Task<IResult>> stage) => RegisterStage(new Stage(stage));
    protected void RegisterStage(Func<MessageContext, Task<IResult>> stage) => RegisterStage(new Stage(stage));
    protected void RegisterStage(Func<MessageContext, IResult> stage) => RegisterStage(new Stage(stage));
}
