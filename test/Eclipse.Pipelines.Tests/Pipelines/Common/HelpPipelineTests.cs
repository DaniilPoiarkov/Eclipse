using Eclipse.Application.Contracts.Telegram;
using Eclipse.Core.Results;
using Eclipse.Pipelines.Pipelines.Common;
using Eclipse.Pipelines.Tests.Fixture;

using FluentAssertions;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

using NSubstitute;

using System.Text;

using Xunit;

namespace Eclipse.Pipelines.Tests.Pipelines.Common;

public class HelpPipelineTests : PipelineTestFixture<HelpPipeline>
{
    private readonly ICommandService _commandService;

    public HelpPipelineTests()
    {
        Localizer[""].ReturnsForAnyArgs(new LocalizedString("Help", "Help"));
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
            new("test", "test")
        };

        _commandService.GetList().Returns(Task.FromResult<IReadOnlyList<CommandDto>>(commands));

        var context = GetContext("/help");

        var result = await Sut.RunNext(context);

        var text = result.As<TextResult>();
        text.Should().NotBeNull();

        var expected = new StringBuilder("Help:")
            .AppendLine()
            .AppendLine()
            .AppendLine("/test - test")
            .ToString();

        text.Message.Should().Be(expected);
        text.ChatId.Should().Be(context.User.Id);
        Sut.IsFinished.Should().BeTrue();
    }
}
