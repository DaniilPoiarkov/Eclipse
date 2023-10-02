using Eclipse.Core.Core;

namespace Eclipse.Core.Builder;

internal class NullCoreDecorator : ICoreDecorator
{
    public Task<IResult> Decorate(Func<MessageContext, CancellationToken, Task<IResult>> execution, MessageContext context, CancellationToken cancellationToken = default)
    {
        return execution(context, cancellationToken);
    }
}
