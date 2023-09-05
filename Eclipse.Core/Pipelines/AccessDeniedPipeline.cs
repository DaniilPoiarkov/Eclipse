using Eclipse.Core.Core;
using Eclipse.Core.Validation;
using Telegram.Bot;

namespace Eclipse.Core.Pipelines;

internal class AccessDeniedPipeline : PipelineBase, IAccessDeniedPipeline
{
    protected readonly ITelegramBotClient BotClient;

    protected IReadOnlyList<ValidationResult> Errors { get; private set; } = new List<ValidationResult>();

    public AccessDeniedPipeline(ITelegramBotClient botClient)
    {
        BotClient = botClient;
    }

    public virtual void SetResults(IEnumerable<ValidationResult> results)
    {
        Errors = results.Where(r => r.IsFailed).ToList();
    }

    protected override void Initialize()
    {
        RegisterStage(ProccedErrors);
    }

    public virtual Task<IResult> ProccedErrors(MessageContext context, CancellationToken cancellationToken = default) => Task.FromResult(Empty());
}
