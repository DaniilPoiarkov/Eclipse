using Eclipse.Core.Builder;
using Eclipse.Core.Core;
using Eclipse.Core.Results;

using Microsoft.Extensions.DependencyInjection;

namespace Eclipse.Core.Pipelines;

public abstract class PipelineBase : Pipeline
{
    public bool IsFinished => _stages.Count == 0;

    private readonly Queue<IStage> _stages = new();

    public PipelineBase()
    {
        Initialize();
    }

    public virtual async Task<IResult> RunNext(MessageContext context, CancellationToken cancellationToken = default)
    {
        var decorations = context.Services.GetServices<ICoreDecorator>();

        Func<MessageContext, CancellationToken, Task<IResult>> execution = RunAsync;

        foreach (var stage in decorations)
        {
            var inner = execution;

            execution = async (context, token) =>
            {
                return await stage.Decorate(inner, context, token);
            };
        }

        return await execution(context, cancellationToken);
    }

    private async Task<IResult> RunAsync(MessageContext context, CancellationToken cancellationToken = default)
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

    protected void FinishPipeline()
    {
        _stages.Clear();
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
