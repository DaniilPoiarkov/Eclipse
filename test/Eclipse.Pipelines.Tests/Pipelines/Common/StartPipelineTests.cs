using Eclipse.Application.Contracts.Configuration;
using Eclipse.Application.Contracts.Users;
using Eclipse.Core.Results;
using Eclipse.Localization.Culture;
using Eclipse.Pipelines.Culture;
using Eclipse.Pipelines.Pipelines.Common;
using Eclipse.Pipelines.Stores.Messages;
using Eclipse.Pipelines.Tests.Fixture;
using Eclipse.Tests.Generators;

using FluentAssertions;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

using NSubstitute;

using Xunit;

namespace Eclipse.Pipelines.Tests.Pipelines.Common;

public sealed class StartPipelineTests : PipelineTestFixture<StartPipeline>
{
    protected override void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton(Substitute.For<ICultureTracker>())
            .AddSingleton(Substitute.For<IUserService>())
            .AddSingleton(Substitute.For<IMessageStore>())
            .AddSingleton(Substitute.For<IOptions<CultureList>>())
            .AddSingleton(Substitute.For<ICurrentCulture>());
    }

    [Fact]
    public async Task RunNext_WhenExecuted_ThenTextReturned()
    {
        var userService = ServiceProvider.GetRequiredService<IUserService>();

        var user = UserGenerator.Get();
        var context = GetContext("/start");

        userService.GetByChatIdAsync(context.ChatId)
            .Returns(new UserDto
            {
                Id = user.Id,
                ChatId = context.ChatId,
                Name = user.Name,
                Surname = user.Surname,
                UserName = user.UserName,
                Culture = user.Culture,
            });

        Localizer["", Arg.Any<object[]>()].ReturnsForAnyArgs(new LocalizedString("{0}", context.User.Name));

        var result = await Sut.RunNext(context);

        var menu = result.As<MenuResult>();

        menu.Should().NotBeNull();
        menu.Message.Should().Be(context.User.Name);
        Sut.IsFinished.Should().BeTrue();
    }
}
