using Eclipse.Core.Pipelines;
using Eclipse.Core.Stores;

using FluentAssertions;

using Xunit;

namespace Eclipse.Core.Tests.Stores;

public abstract class PipelineStoreTests
{
    private readonly IPipelineStore _sut;

    public PipelineStoreTests()
    {
        _sut = Initialize();
    }

    internal protected abstract IPipelineStore Initialize();

    [Fact]
    public async Task Set_WhenMessageIdSpecified_ThenSetsPipelines()
    {
        await _sut.Set(1, 1, new TestPipeline());
        await _sut.Set(1, 2, new TestPipeline());

        var pipeline1 = await _sut.Get(1, 1);
        var pipeline2 = await _sut.Get(1, 2);

        pipeline1.Should().NotBeNull();
        pipeline1!.StagesLeft.Should().Be(2);

        pipeline2.Should().NotBeNull();
        pipeline2!.StagesLeft.Should().Be(2);
    }

    [Fact]
    public async Task Set_WhenMessageIdSpecified_AndPipelineHasSkippedStages_ThenSetsPipelines()
    {
        var pipeline = new TestPipeline();
        pipeline.SkipStage();

        var stagesLeft = pipeline.StagesLeft;

        await _sut.Set(2, 1, pipeline);

        var result = await _sut.Get(2, 1);

        result.Should().NotBeNull();
        result!.StagesLeft.Should().Be(stagesLeft);
    }
}

internal sealed class TestPipeline : PipelineBase
{
    protected override void Initialize()
    {
        RegisterStage(_ => Text("Test1"));
        RegisterStage(_ => Text("Test2"));
    }
}
