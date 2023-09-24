using Eclipse.Application.Contracts.Notifications;
using Eclipse.Core.Attributes;
using Eclipse.Core.Core;

namespace Eclipse.Pipelines.Pipelines;

[Route("Test")]
internal class TestPipeline : EclipsePipelineBase
{
    private readonly INotificationService _notificationService;

    public TestPipeline(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    protected override void Initialize()
    {
        RegisterStage(Test);
    }

    private async Task<IResult> Test(MessageContext context, CancellationToken token)
    {
        await _notificationService.AddTestJob(context.ChatId, TimeZoneInfo.Utc);
        return Empty();
    }
}
