using Eclipse.Application.Contracts.Telegram;
using Eclipse.Application.Telegram;
using Eclipse.Localization.Exceptions;

using FluentAssertions;

using NSubstitute;

using Telegram.Bot;

using Xunit;

namespace Eclipse.Application.Tests.Telegram;

public class TelegramServiceTests
{
    private readonly ITelegramService _sut;

    public TelegramServiceTests()
    {
        var botClient = Substitute.For<ITelegramBotClient>();
        _sut = new TelegramService(botClient, new SendMessageModelValidator());
    }

    [Fact]
    public async Task Send_WhenMessageValid_ThenNoExceptionThrown()
    {
        var model = new SendMessageModel
        {
            ChatId = 1,
            Message = "test",
        };

        var action = async () =>
        {
            await _sut.Send(model);
        };

        await action.Should().NotThrowAsync();
    }

    [Fact]
    public async Task Send_WhenModelHasDefaultChatId_ThenExceptionThrown()
    {
        var model = new SendMessageModel
        {
            ChatId = 0,
            Message = "test",
        };

        var action = async () =>
        {
            await _sut.Send(model);
        };

        await action.Should().ThrowAsync<LocalizedException>();
    }

    [Fact]
    public async Task Send_WhenMessageIsEmpty_ThenExceptionThrown()
    {

        var model = new SendMessageModel
        {
            ChatId = 0,
            Message = string.Empty,
        };

        var action = async () =>
        {
            await _sut.Send(model);
        };

        await action.Should().ThrowAsync<LocalizedException>();
    }
}
