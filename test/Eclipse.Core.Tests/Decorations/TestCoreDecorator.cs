using Eclipse.Core.Builder;
using Eclipse.Core.Core;

namespace Eclipse.Core.Tests.Decorations;

internal class TestCoreDecorator : IPipelineExecutionDecorator
{
    internal event EventHandler? Touched;

    public Task<IResult> Decorate(Func<MessageContext, CancellationToken, Task<IResult>> execution, MessageContext context, CancellationToken cancellationToken = default)
    {
        Touched?.Invoke(this, EventArgs.Empty);
        return execution(context, cancellationToken);
    }
}
