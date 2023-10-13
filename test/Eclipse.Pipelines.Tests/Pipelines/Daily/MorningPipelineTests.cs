using Eclipse.Application.Contracts.Telegram.Messages;
using Eclipse.Core.Results;
using Eclipse.Pipelines.Pipelines.Daily;
using Eclipse.Pipelines.Tests.Fixture;

using FluentAssertions;

using Microsoft.Extensions.DependencyInjection;

using NSubstitute;

using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

using Xunit;

namespace Eclipse.Pipelines.Tests.Pipelines.Daily;

public class MorningPipelineTests : PipelineTestFixture<MorningPipeline>
{
    public MorningPipelineTests()
    {
        Localizer["Pipelines:Morning:AskMood"].Returns("AskMood");
        Localizer["Pipelines:Morning:GoodMood"].Returns("Good");
        Localizer["Pipelines:Morning:BadMood"].Returns("Bad");
        Localizer["Pipelines:Morning:NotDefined"].Returns("NotDefined");
    }

    protected override void ConfigureServices(IServiceCollection services)
    {
        var messageStore = Substitute.For<IMessageStore>();

        services.AddSingleton(messageStore);
        base.ConfigureServices(services);
    }

    [Fact]
    public async Task WhenUserHasNoMessageInStore_ThenReturnesMenuWithChoices_AndWhenRecievedGoodMood_ReturnesGoodText()
    {
        var context = GetContext("/daily_morning");
        var menuResult = await Sut.RunNext(context);

        var isFinished = Sut.IsFinished;

        context = GetContext("👍");
        var textResult = await Sut.RunNext(context);


        menuResult.As<MenuResult>().Should().NotBeNull();
        AssertResult(menuResult, assertion => assertion.FieldHasValue("_message", "AskMood"));
        isFinished.Should().BeFalse();

        textResult.As<TextResult>().Should().NotBeNull();
        AssertResult(textResult, assertion => assertion.FieldHasValue("_message", "Good"));
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

        messageStore.GetMessage(default!).ReturnsForAnyArgs(message);
        var context = GetContext("/daily_morning");

        var multipleResult = await Sut.RunNext(context);
        var isFinished = Sut.IsFinished;

        context = GetContext("test");
        var textResult = await Sut.RunNext(context);

        multipleResult.As<MultipleActionsResult>().Should().NotBeNull();
        isFinished.Should().BeFalse();

        Sut.IsFinished.Should().BeTrue();
        textResult.As<TextResult>().Should().NotBeNull();
        AssertResult(textResult, assertion => assertion.FieldHasValue("_message", "NotDefined"));
    }
}
