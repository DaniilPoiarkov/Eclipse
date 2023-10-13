using Eclipse.Core.Results;
using Eclipse.Pipelines.Pipelines.Common;
using Eclipse.Pipelines.Tests.Fixture;

using FluentAssertions;

using NSubstitute;

using Xunit;

namespace Eclipse.Pipelines.Tests.Pipelines.Common;

public class AboutPipelineTests : PipelineTestFixture<AboutPipeline>
{
    public AboutPipelineTests()
    {
        Localizer[""].ReturnsForAnyArgs("{name}");
    }

    [Fact]
    public async Task WhenExecuted_ThenTextRetuned()
    {
        var context = GetContext("/about");

        var result = await Sut.RunNext(context);

        result.As<TextResult>().Should().NotBeNull();
        
        AssertResult(result, assertion => assertion
            .FieldHasValue("_message", context.User.Name));

        Sut.IsFinished.Should().BeTrue();
    }
}
