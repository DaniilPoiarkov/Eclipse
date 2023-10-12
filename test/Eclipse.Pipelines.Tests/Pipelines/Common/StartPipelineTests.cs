using Eclipse.Core.Results;
using Eclipse.Pipelines.Pipelines.Common;
using Eclipse.Pipelines.Tests.Fixture;
using Eclipse.Tests.Generators;

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
        var context = MessageContextGenerator.Generate("/start");
        context.Services = ServiceProvider;

        var result = await Sut.RunNext(context);

        var menu = result.As<MenuResult>();
        
        menu.Should().NotBeNull();

        AssertResult(menu, assertion => assertion.FieldHasValue("_message", context.User.Name));
    }
}
