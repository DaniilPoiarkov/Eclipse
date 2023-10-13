using Eclipse.Core.Results;
using Eclipse.Pipelines.Pipelines.Common;
using Eclipse.Pipelines.Tests.Fixture;

using FluentAssertions;

using NSubstitute;

using Xunit;

namespace Eclipse.Pipelines.Tests.Pipelines.Common;

public class StartPipelineTests : PipelineTestFixture<StartPipeline>
{
    public StartPipelineTests()
    {
        Localizer[""].ReturnsForAnyArgs("{name}");
    }

    [Fact]
    public async Task RunNext_WhenExecuted_ThenTextReturned()
    {
        var context = GetContext("/start");

        var result = await Sut.RunNext(context);

        result.As<MenuResult>().Should().NotBeNull();
        AssertResult(result, assertion => assertion.FieldHasValue("_message", context.User.Name));
        Sut.IsFinished.Should().BeTrue();
    }
}
