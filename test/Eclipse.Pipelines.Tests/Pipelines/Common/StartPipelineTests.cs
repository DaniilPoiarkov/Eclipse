using Eclipse.Core.Core;
using Eclipse.Core.Models;
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
        var user = new TelegramUser(1, "Name", "Surname", "Username");
        var context = new MessageContext(1, "/start", user)
        {
            Services = ServiceProvider
        };

        var result = await Sut.RunNext(context);

        var menu = result.As<MenuResult>();
        
        menu.Should().NotBeNull();

        AssertResult(menu, assertion => assertion.FieldHasValue("_message", user.Name));
    }
}
