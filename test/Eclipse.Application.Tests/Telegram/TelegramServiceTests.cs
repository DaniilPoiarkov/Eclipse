using Eclipse.Application.Contracts.Telegram;
using Eclipse.Application.Telegram;
using Eclipse.Common.Results;

using FluentAssertions;

using NSubstitute;

using Telegram.Bot;

using Xunit;

namespace Eclipse.Application.Tests.Telegram;

public sealed class TelegramServiceTests
{
    private readonly ITelegramService _sut;

    private static readonly string _errorCode = "Telegram.Send";

    public TelegramServiceTests()
    {
        var botClient = Substitute.For<ITelegramBotClient>();
        _sut = new TelegramService(botClient);
    }

    [Fact]
    public async Task Send_WhenMessageValid_ThenNoExceptionThrown()
    {
        var model = new SendMessageModel
        {
            ChatId = 1,
            Message = "test",
        };

        var result = await _sut.Send(model);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Send_WhenModelHasDefaultChatId_ThenFailureResultReturned()
    {
        var expectedError =  Error.Validation(_errorCode, "Telegram:InvalidChatId");

        var model = new SendMessageModel
        {
            ChatId = 0,
            Message = "test",
        };

        var result = await _sut.Send(model);

        result.IsSuccess.Should().BeFalse();

        var error = result.Error;
        error.Code.Should().Be(expectedError.Code);
        error.Description.Should().Be(expectedError.Description);
        error.Args.Should().BeEquivalentTo(expectedError.Args);
    }

    [Fact]
    public async Task Send_WhenMessageIsEmpty_ThenFailureResultReturned()
    {
        var expectedError = Error.Validation(_errorCode, "Telegram:MessageCannotBeEmpty");

        var model = new SendMessageModel
        {
            ChatId = 0,
            Message = string.Empty,
        };

        var result = await _sut.Send(model);

        result.IsSuccess.Should().BeFalse();

        var error = result.Error;
        error.Code.Should().Be(expectedError.Code);
        error.Description.Should().Be(expectedError.Description);
        error.Args.Should().BeEquivalentTo(expectedError.Args);
    }
}
