using Eclipse.Core.Pipelines;
using Eclipse.Core.Provider;
using Eclipse.Core.Tests.Pipelines;

using FluentAssertions;

using Microsoft.Extensions.DependencyInjection;

using Telegram.Bot.Types;

using Xunit;

namespace Eclipse.Core.Tests;

public class PipelineProviderTests
{
    private readonly PipelineProvider _pipelineProvider;

    public PipelineProviderTests()
    {
        var serviceProvider = new ServiceCollection()
            .AddCoreModule()
            .AddSingleton<PipelineBase, Test1Pipeline>()
            .AddSingleton<PipelineBase, Test2Pipeline>()
            .AddSingleton<PipelineBase, TestAccessFailsPipeline>()
            .AddSingleton<PipelineBase, TestAccessPassedPipeline>()
            .BuildServiceProvider();

        var pipelines = serviceProvider.GetServices<PipelineBase>();
        var handlers = serviceProvider.GetServices<IProviderHandler>();

        _pipelineProvider = new PipelineProvider(serviceProvider, pipelines, handlers);
    }

    [Fact]
    public void Get_WhenPipelineCanBeRetrieved_ThenPipelineReturned()
    {
        var pipeline = _pipelineProvider.Get(new Update { Message = new Message { Text = "Test1" } });
        pipeline.As<Test1Pipeline>().Should().NotBeNull();
    }

    [Fact]
    public void Get_WhenPipelineCannotBeRetrieved_ThenNotFoundPipelineReturnes()
    {
        var pipeline = _pipelineProvider.Get(new Update { Message = new Message { Text = "Test3" } });
        pipeline.As<INotFoundPipeline>().Should().NotBeNull();
    }

    [Fact]
    public void Get_WhenRouteIsNull_ThenNotFoundPipelineReturned()
    {
        var pipeline = _pipelineProvider.Get(new Update { Message = new Message { Text = string.Empty } });
        pipeline.As<INotFoundPipeline>().Should().NotBeNull();
    }

    [Fact]
    public void Get_WhenPipelineHasValidationAttribute_AndValidationFailes_ThenAccessDeniedPipelineReturned()
    {
        var pipeline = _pipelineProvider.Get(new Update { Message = new Message { Text = "TestAccessFails" } });
        pipeline.As<IAccessDeniedPipeline>().Should().NotBeNull();
    }

    [Fact]
    public void Get_WhenSpecifiedByCommand_ThenPipelineReturned()
    {
        var pipeline = _pipelineProvider.Get(new Update { Message = new Message { Text = "/test1" } });
        pipeline.As<Test1Pipeline>().Should().NotBeNull();
    }

    [Fact]
    public void Get_WhenPipelineHasValidationAttribute_AndValidationPasses_ThenPipelineReturned()
    {
        var pipeline = _pipelineProvider.Get(new Update { Message = new Message { Text = "TestAccessPassed" } });
        pipeline.As<TestAccessPassedPipeline>().Should().NotBeNull();
    }
}
