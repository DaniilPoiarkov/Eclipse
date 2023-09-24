using Eclipse.Core.Core;
using Eclipse.Core.Models;
using Eclipse.Core.Pipelines;
using Eclipse.Core.Tests.Pipelines;

using FluentAssertions;

using Microsoft.Extensions.DependencyInjection;

using NSubstitute;

namespace Eclipse.Core.Tests;

public class PipelineProviderTests
{
    private readonly IPipelineProvider _pipelineProvider;

    public PipelineProviderTests()
    {
        var pipelines = new PipelineBase[] { new Test1Pipeline(), new Test2Pipeline() };
        var serviceProvider = new ServiceCollection()
            .AddSingleton<Test1Pipeline>()
            .AddSingleton<Test2Pipeline>()
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
}
