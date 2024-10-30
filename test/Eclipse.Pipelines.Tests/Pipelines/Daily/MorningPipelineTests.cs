using Eclipse.Application.Contracts.MoodRecords;
using Eclipse.Application.Contracts.Users;
using Eclipse.Core.Results;
using Eclipse.Pipelines.Pipelines.Daily.MoodRecords;
using Eclipse.Pipelines.Stores.Messages;
using Eclipse.Pipelines.Tests.Fixture;
using Eclipse.Tests.Generators;

using FluentAssertions;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

using NSubstitute;

using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

using Xunit;

namespace Eclipse.Pipelines.Tests.Pipelines.Daily;

public class MorningPipelineTests : PipelineTestFixture<AddMoodRecordPipeline>
{
    public MorningPipelineTests()
    {
        Localizer["Pipelines:Morning:AskMood"].Returns(new LocalizedString("Pipelines:Morning:AskMood", "AskMood"));
        Localizer["Pipelines:Morning:GoodMood"].Returns(new LocalizedString("Pipelines:Morning:GoodMood", "Good"));
        Localizer["Pipelines:Morning:BadMood"].Returns(new LocalizedString("Pipelines:Morning:BadMood", "Bad"));
        Localizer["Pipelines:Morning:NotDefined"].Returns(new LocalizedString("Pipelines:Morning:NotDefined", "NotDefined"));
    }

    protected override void ConfigureServices(IServiceCollection services)
    {
        services
            .AddSingleton(Substitute.For<IMessageStore>())
            .AddSingleton(Substitute.For<IMoodRecordsService>())
            .AddSingleton(Substitute.For<IUserService>());

        base.ConfigureServices(services);
    }

    [Theory]
    [InlineData("5️⃣", "Good")]
    [InlineData("4️⃣", "Good")]
    [InlineData("3️⃣", "Bad")]
    [InlineData("2️⃣", "Bad")]
    [InlineData("1️⃣", "Bad")]
    public async Task WhenUserHasNoMessageInStore_ThenReturnesMenuWithChoices_AndWhenRecievedGoodMood_ReturnesGoodText(string answer, string message)
    {
        var userService = GetService<IUserService>();

        var user = UserDtoGenerator.Get();

        userService.GetByChatIdAsync(Arg.Any<long>()).Returns(user);

        var context = GetContext("/daily_morning");
        var menuResult = await Sut.RunNext(context);

        var isFinished = Sut.IsFinished;

        context = GetContext(answer);
        var textResult = await Sut.RunNext(context);


        var menu = menuResult.As<MenuResult>();

        menu.Should().NotBeNull();
        menu.Message.Should().Be("AskMood");
        isFinished.Should().BeFalse();

        var text = textResult.As<TextResult>();
        text.Should().NotBeNull();
        text.Message.Should().Be(message);
        Sut.IsFinished.Should().BeTrue();
    }

    [Fact]
    public async Task WhenUserHasMessage_ThenMultipleResultReturned()
    {
        var messageStore = ServiceProvider.GetRequiredService<IMessageStore>();

        var message = new Message
        {
            ReplyMarkup = new InlineKeyboardMarkup(new InlineKeyboardButton("test"))
        };

        messageStore.GetOrDefaultAsync(default!).ReturnsForAnyArgs(message);
        var context = GetContext("/daily_morning");

        // First message act
        var firstStepResult = await Sut.RunNext(context);
        var isFinished = Sut.IsFinished;

        // Second message act
        context = GetContext("test");
        var secondStepResult = await Sut.RunNext(context);

        // First message assertion
        var firstStepTypedResult = firstStepResult.As<MultipleActionsResult>();
        firstStepTypedResult.Should().NotBeNull();
        firstStepTypedResult.Results.Count.Should().Be(2);
        isFinished.Should().BeFalse();

        // Second message assertion
        var secondStepTypedResult = secondStepResult.As<MultipleActionsResult>();
        secondStepTypedResult.Should().NotBeNull();
        secondStepTypedResult.Results.Count.Should().Be(2);

        var textResult = secondStepTypedResult.Results.FirstOrDefault(r => r is TextResult);
        textResult.Should().NotBeNull();
        textResult.As<TextResult>().Message.Should().Be("NotDefined");
        Sut.IsFinished.Should().BeTrue();
    }
}
