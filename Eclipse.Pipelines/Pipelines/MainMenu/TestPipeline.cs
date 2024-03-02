using Eclipse.Common.EventBus;
using Eclipse.Core.Attributes;
using Eclipse.Domain.IdentityUsers.Events;
using Eclipse.Pipelines.Attributes;

namespace Eclipse.Pipelines.Pipelines.MainMenu;

[AdminOnly]
[Route("Test", "/test")]
internal sealed class TestPipeline : EclipsePipelineBase
{
    private readonly IEventBus _eventBus;

    public TestPipeline(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    protected override void Initialize()
    {
        RegisterStage(context => Text("ONE MORE TIME"));

        RegisterStage(async (context, token) =>
        {
            await _eventBus.Publish(new TestDomainEvent(context.ChatId), token);
            return Text("OK");
        });
    }
}
