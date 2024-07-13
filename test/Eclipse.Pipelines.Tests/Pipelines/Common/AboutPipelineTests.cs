using Eclipse.Core.Results;
using Eclipse.Pipelines.Pipelines.Common;
using Eclipse.Pipelines.Tests.Fixture;

using FluentAssertions;

using Microsoft.Extensions.Localization;

using NSubstitute;

using Xunit;

namespace Eclipse.Pipelines.Tests.Pipelines.Common;

public class AboutPipelineTests : PipelineTestFixture<AboutPipeline>
{
    public AboutPipelineTests()
    {
        Localizer[""].ReturnsForAnyArgs(new LocalizedString("{name}", "{name}"));
    }

    [Fact]
    public async Task WhenExecuted_ThenTextRetuned()
    {
        var context = GetContext("/about");

        var result = await Sut.RunNext(context);

        var text = result.As<TextResult>();

        text.Should().NotBeNull();
        text.Message.Should().Be(context.User.Name);
        text.ChatId.Should().Be(context.User.Id);
        Sut.IsFinished.Should().BeTrue();
    }
}
