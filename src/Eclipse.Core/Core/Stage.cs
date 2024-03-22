namespace Eclipse.Core.Core;

internal sealed class Stage : IStage
{
    private readonly Func<MessageContext, CancellationToken, Task<IResult>> _stage;

    public Stage(Func<MessageContext, CancellationToken, Task<IResult>> stage)
    {
        _stage = stage;
    }

    public Stage(Func<MessageContext, Task<IResult>> stage)
        : this((context, _) => stage(context)) { }

    public Stage(Func<MessageContext, IResult> stage)
        : this((context) => Task.FromResult(stage(context))) { }

    public Task<IResult> RunAsync(MessageContext context, CancellationToken cancellationToken = default) => _stage(context, cancellationToken);
}
