using Eclipse.Core.Core;

namespace Eclipse.Core.Builder;

public interface IPipelineExecutionDecorator
{
    /// <summary>
    /// Decorates <a cref="Pipelines.PipelineBase.RunNext(MessageContext, CancellationToken)"></a> execution
    /// </summary>
    /// <param name="execution">Next execution step</param>
    /// <param name="context">Execution context</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IResult> Decorate(Func<MessageContext, CancellationToken, Task<IResult>> execution, MessageContext context, CancellationToken cancellationToken = default);
}
