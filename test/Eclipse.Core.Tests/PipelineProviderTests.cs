using Eclipse.Core.Context;
using Eclipse.Core.CurrentUser;
using Eclipse.Core.Pipelines;
using Eclipse.Core.Provider;
using Eclipse.Core.Tests.Pipelines;

using FluentAssertions;

using Microsoft.Extensions.DependencyInjection;

using NSubstitute;

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

        var currentUser = Substitute.For<ICurrentTelegramUser>();
        currentUser.GetCurrentUser().Returns(new TelegramUser());

        var pipelines = serviceProvider.GetServices<PipelineBase>();

        _pipelineProvider = new PipelineProvider(pipelines, serviceProvider, currentUser);
    }

    [Fact]
    public void Get_WhenPipelineCanBeRetrieved_ThenPipelineReturned()
    {
        var pipeline = _pipelineProvider.Get("Test1");
        pipeline.As<Test1Pipeline>().Should().NotBeNull();
    }

    [Fact]
    public void Get_WhenPipelineCannotBeRetrieved_ThenNotFoundPipelineReturnes()
    {
        var pipeline = _pipelineProvider.Get("Test3");
        pipeline.As<INotFoundPipeline>().Should().NotBeNull();
    }

    [Fact]
    public void Get_WhenRouteIsNull_ThenNotFoundPipelineReturned()
    {
        var pipeline = _pipelineProvider.Get(string.Empty);
        pipeline.As<INotFoundPipeline>().Should().NotBeNull();
    }

    [Fact]
    public void Get_WhenPipelineHasValidationAttribute_AndValidationFailes_ThenAccessDeniedPipelineReturned()
    {
        var pipeline = _pipelineProvider.Get("TestAccessFails");
        pipeline.As<IAccessDeniedPipeline>().Should().NotBeNull();
    }

    [Fact]
    public void Get_WhenSpecifiedByCommand_ThenPipelineReturned()
    {
        var pipeline = _pipelineProvider.Get("/test1");
        pipeline.As<Test1Pipeline>().Should().NotBeNull();
    }

    [Fact]
    public void Get_WhenPipelineHasValidationAttribute_AndValidationPasses_ThenPipelineReturned()
    {
        var pipeline = _pipelineProvider.Get("TestAccessPassed");
        pipeline.As<TestAccessPassedPipeline>().Should().NotBeNull();
    }
}
