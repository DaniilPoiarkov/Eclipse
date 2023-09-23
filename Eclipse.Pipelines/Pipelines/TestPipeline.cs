using Eclipse.Core.Attributes;
using Eclipse.Core.Core;
using Eclipse.Infrastructure.Quartz;

namespace Eclipse.Pipelines.Pipelines;

[Route("Test")]
internal class TestPipeline : EclipsePipelineBase
{
    private readonly IEclipseScheduler _scheduler;

    public TestPipeline(IEclipseScheduler scheduler)
    {
        _scheduler = scheduler;
    }

    protected override void Initialize()
    {
        RegisterStage(Test);
    }

    private async Task<IResult> Test(MessageContext context, CancellationToken token)
    {
        await _scheduler.Test(context.ChatId);
        return Empty();
    }
}
