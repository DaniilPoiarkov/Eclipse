using Eclipse.Core.Builder;
using Eclipse.Core.Core;
using Eclipse.Core.Results;

using Microsoft.Extensions.DependencyInjection;

namespace Eclipse.Core.Pipelines;

public abstract class PipelineBase : Pipeline
{
    public bool IsFinished => StagesLeft == 0;

    public int StagesLeft => _stages.Count;

    private readonly Queue<IStage> _stages = new();

    public PipelineBase()
    {
        Initialize();
    }

    public virtual Task<IResult> RunNext(MessageContext context, CancellationToken cancellationToken = default)
    {
        var decorations = context.Services.GetServices<IPipelineExecutionDecorator>();

        Func<MessageContext, CancellationToken, Task<IResult>> execution = RunAsync;

        foreach (var stage in decorations)
        {
            var inner = execution;

            execution = (context, token) =>
            {
                return stage.Decorate(inner, context, token);
            };
        }

        return execution(context, cancellationToken);
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

        var result = await next.RunAsync(context, cancellationToken);

        if (result is ResultBase resultBase)
        {
            resultBase.ChatId = context.ChatId;
        }

        if (result is RedirectResult redirect)
        {
            var pipeline = context.Services.GetService(redirect.PipelineType) as PipelineBase
                ?? throw new PipelineResolvingException($"Pipeline of type {redirect.PipelineType.FullName} can not be resolved.");

            return await pipeline.RunNext(context, cancellationToken);
        }

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

    public void SkipStage()
    {
        _stages.Dequeue();
    }
}
