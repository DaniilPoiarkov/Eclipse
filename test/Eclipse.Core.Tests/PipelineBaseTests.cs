using Eclipse.Core.Builder;
using Eclipse.Core.Core;
using Eclipse.Core.Models;
using Eclipse.Core.Pipelines;
using Eclipse.Core.Tests.Decorations;
using Eclipse.Core.Tests.Pipelines;

using FluentAssertions;

using Microsoft.Extensions.DependencyInjection;

using Xunit;

namespace Eclipse.Core.Tests;

public class PipelineBaseTests
{
    private readonly Test1Pipeline _pipeline1 = new();

    private readonly Test2Pipeline _pipeline2 = new();

    private readonly IServiceCollection _services;

    private MessageContext MessageContext
    {
        get
        {
            return new MessageContext(0, string.Empty, new TelegramUser(), _services.BuildServiceProvider());
        }
    }

    public PipelineBaseTests()
    {
        _services = new ServiceCollection()
            .AddCoreModule()
            .AddTransient<PipelineBase, Test1Pipeline>()
            .AddTransient<PipelineBase, Test2Pipeline>();
    }

    [Fact]
    public async Task RunNext_WhenTwoStepsRegistered_ThenTwoStepsCanBeProcessed()
    {
        _ = await _pipeline1.RunNext(MessageContext);
        var isFinished = _pipeline1.IsFinished;

        _ = await _pipeline1.RunNext(MessageContext);

        isFinished.Should().BeFalse();
        _pipeline1.IsFinished.Should().BeTrue();
    }

    [Fact]
    public async Task FinishPipeline_WhenThreeStepsRegisteredAndPipelineForcedToFinishInSecondStep_ThenOnlyTwoStepsProcceded()
    {
        await _pipeline2.RunNext(MessageContext);
        await _pipeline2.RunNext(MessageContext);

        _pipeline2.IsFinished.Should().BeTrue();
    }

    [Fact]
    public async Task RunNext_WhenPipelineDecorated_ThenDecorationProcceded()
    {
        var decorator = new TestCoreDecorator();

        var touched = 0;
        var runs = 0;

        decorator.Touched += (_, _) => touched++;

        _services.AddSingleton<IPipelineExecutionDecorator>(decorator);

        while (!_pipeline1.IsFinished)
        {
            _ = await _pipeline1.RunNext(MessageContext);
            runs++;
        }

        touched.Should().Be(runs);
    }

    [Fact]
    public async Task RunNext_WhenPipelineHasMultipleDecorations_ThenAllDecorationsProcceded()
    {
        var decorator1 = new TestCoreDecorator();
        var decorator2 = new TestCoreDecorator();

        var touched = 0;

        decorator1.Touched += (_, _) => touched++;
        decorator2.Touched += (_, _) => touched--;

        _services
            .AddSingleton<IPipelineExecutionDecorator>(decorator1)
            .AddSingleton<IPipelineExecutionDecorator>(decorator2);

        while (!_pipeline1.IsFinished)
        {
            _ = await _pipeline1.RunNext(MessageContext);
        }

        touched.Should().Be(0);
    }
}
