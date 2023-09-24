using Eclipse.Core.Core;
using Eclipse.Core.Models;
using Eclipse.Core.Pipelines;
using Eclipse.Core.Tests.Pipelines;

using Microsoft.Extensions.DependencyInjection;

namespace Eclipse.Core.Tests;

public class PipelineProviderTests
{
    private readonly IPipelineProvider _pipelineProvider;

    public PipelineProviderTests()
    {
        var pipelines = new PipelineBase[]
        {
            new Test1Pipeline(),
            new Test2Pipeline(),
            new TestAccessPipeline(),
        };

        var serviceProvider = new ServiceCollection()
            .AddCoreModule()
            .AddSingleton<Test1Pipeline>()
            .AddSingleton<Test2Pipeline>()
            .AddSingleton<TestAccessPipeline>()
            .BuildServiceProvider();

        var currentUser = Substitute.For<ICurrentTelegramUser>();
        currentUser.GetCurrentUser().Returns(new TelegramUser());

        _pipelineProvider = new PipelineProvider(pipelines, serviceProvider, currentUser );
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
        var pipeline = _pipelineProvider.Get("TestAccess");
        pipeline.As<IAccessDeniedPipeline>().Should().NotBeNull();
    }

    [Fact]
    public void Get_WhenSpecifiedByCommand_ThenPipelineReturned()
    {
        var pipeline = _pipelineProvider.Get("/test1");
        pipeline.As<Test1Pipeline>().Should().NotBeNull();
    }
}
