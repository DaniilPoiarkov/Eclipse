namespace Eclipse.Core.Core;

public interface IStage
{
    Task<IResult> RunAsync(MessageContext context, CancellationToken cancellationToken = default);
}
