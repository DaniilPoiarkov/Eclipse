using Eclipse.Core.Context;
using Eclipse.Core.Results;

namespace Eclipse.Core.Builder;

internal sealed class NullPipelineExecutionDecorator : IPipelineExecutionDecorator
{
    public Task<IResult> Decorate(Func<MessageContext, CancellationToken, Task<IResult>> execution, MessageContext context, CancellationToken cancellationToken = default)
    {
        return execution(context, cancellationToken);
    }
}
