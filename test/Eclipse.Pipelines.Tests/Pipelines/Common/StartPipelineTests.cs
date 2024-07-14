using Eclipse.Core.Results;
using Eclipse.Pipelines.Pipelines.Common;
using Eclipse.Pipelines.Tests.Fixture;

using FluentAssertions;

using Microsoft.Extensions.Localization;

using NSubstitute;

using Xunit;

namespace Eclipse.Pipelines.Tests.Pipelines.Common;

public class StartPipelineTests : PipelineTestFixture<StartPipeline>
{
    public StartPipelineTests()
    {
        Localizer[""].ReturnsForAnyArgs(new LocalizedString("{name}", "{name}"));
    }

    [Fact]
    public async Task RunNext_WhenExecuted_ThenTextReturned()
    {
        var context = GetContext("/start");

        var result = await Sut.RunNext(context);

        var menu = result.As<MenuResult>();

        menu.Should().NotBeNull();
        menu.Message.Should().Be(context.User.Name);
        Sut.IsFinished.Should().BeTrue();
    }
}
