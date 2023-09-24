using Eclipse.Core.Core;
using Eclipse.Core.Models;
using Eclipse.Core.Tests.Pipelines;

namespace Eclipse.Core.Tests;

public class PipelineBaseTests
{
    private readonly Test1Pipeline _pipeline1 = new();

    private readonly Test2Pipeline _pipeline2 = new();

    [Fact]
    public async Task RunNext_WhenTwoStepsRegistered_ThenTwoStepsCanBeProcessed()
    {
        var messageContext = new MessageContext(0, string.Empty, new TelegramUser());

        _ = await _pipeline1.RunNext(messageContext);
        var isFinished = _pipeline1.IsFinished;

        _ = await _pipeline1.RunNext(messageContext);

        isFinished.Should().BeFalse();
        _pipeline1.IsFinished.Should().BeTrue();
    }

    [Fact]
    public async Task FinishPipeline_WhenThreeStepsRegisteredAndPipelineForcedToFinishInSecondStep_ThenOnlyTwoStepsProcceded()
    {
        var messageContext = new MessageContext(0, string.Empty, new TelegramUser());

        await _pipeline2.RunNext(messageContext);
        await _pipeline2.RunNext(messageContext);

        _pipeline2.IsFinished.Should().BeTrue();
    }
}
