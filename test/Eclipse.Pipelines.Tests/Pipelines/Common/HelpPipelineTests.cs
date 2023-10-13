using Eclipse.Application.Contracts.Telegram.Commands;
using Eclipse.Core.Results;
using Eclipse.Pipelines.Pipelines.Common;
using Eclipse.Pipelines.Tests.Fixture;

using FluentAssertions;

using Microsoft.Extensions.DependencyInjection;

using NSubstitute;

using Xunit;

namespace Eclipse.Pipelines.Tests.Pipelines.Common;

public class HelpPipelineTests : PipelineTestFixture<HelpPipeline>
{
    private readonly ICommandService _commandService;
    
    public HelpPipelineTests()
    {
        Localizer[""].ReturnsForAnyArgs("Help");
        _commandService = ServiceProvider.GetRequiredService<ICommandService>();
    }

    protected override void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton(Substitute.For<ICommandService>());
        base.ConfigureServices(services);
    }

    [Fact]
    public async Task RunNext_WhenCalled_ThenCommandsListReturned()
    {
        var commands = new List<CommandDto>()
        {
            new CommandDto
            {
                Command = "test",
                Description = "test",
            }
        };

        _commandService.GetList().Returns(Task.FromResult<IReadOnlyList<CommandDto>>(commands));

        var context = GetContext("/help");

        var result = await Sut.RunNext(context);

        var text = result.As<TextResult>();
        text.Should().NotBeNull();

        AssertResult(text, assertion => assertion.FieldHasValue("_message", "Help:\r\n\r\n/test - test\r\n"));
        Sut.IsFinished.Should().BeTrue();
    }
}
