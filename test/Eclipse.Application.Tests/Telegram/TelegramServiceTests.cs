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
    private readonly TelegramService _sut;

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

    [Theory]
    [InlineData(0, "test", "InvalidChatId")]
    [InlineData(0, "", "MessageCannotBeEmpty")]
    public async Task Send_WhenModelInvalid_ThenFailureResultReturned(long chatId, string message, string errorCode)
    {
        var expectedError = Error.Validation(_errorCode, $"Telegram:{errorCode}");

        var model = new SendMessageModel
        {
            ChatId = chatId,
            Message = message,
        };

        var result = await _sut.Send(model);

        result.IsSuccess.Should().BeFalse();

        var error = result.Error;
        error.Code.Should().Be(expectedError.Code);
        error.Description.Should().Be(expectedError.Description);
        error.Args.Should().BeEquivalentTo(expectedError.Args);
    }
}
