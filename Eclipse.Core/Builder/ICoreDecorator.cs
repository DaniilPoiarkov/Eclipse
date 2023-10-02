using Eclipse.Core.Core;

namespace Eclipse.Core.Builder;

public interface ICoreDecorator
{
    Task<IResult> Decorate(Func<MessageContext, CancellationToken, Task<IResult>> execution, MessageContext context, CancellationToken cancellationToken = default);
}
