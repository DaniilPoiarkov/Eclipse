using Eclipse.Application.Contracts.Telegram;
using Eclipse.Application.Telegram;
using Eclipse.Common.Results;

using FluentAssertions;

using Microsoft.Extensions.Configuration;

using NSubstitute;
using NSubstitute.ExceptionExtensions;

using Telegram.Bot;
using Telegram.Bot.Requests;

using Xunit;

namespace Eclipse.Application.Tests.Telegram;

public sealed class TelegramServiceTests
{
    private readonly ITelegramBotClient _botClient;

    private readonly TelegramService _sut;

    private static readonly string _errorSendCode = "Telegram.Send";

    private static readonly string _errorWebhookCode = "Telegram.Webhook";

    public TelegramServiceTests()
    {
        _botClient = Substitute.For<ITelegramBotClient>();
        var configuration = Substitute.For<IConfiguration>();

        _sut = new TelegramService(_botClient, configuration);
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
        var expectedError = Error.Validation(_errorSendCode, $"Telegram:{errorCode}");

        var model = new SendMessageModel
        {
            ChatId = chatId,
            Message = message,
        };

        var result = await _sut.Send(model);

        result.IsSuccess.Should().BeFalse();

        result.Error.Should().BeEquivalentTo(expectedError);
    }

    [Theory]
    [InlineData("")]
    [InlineData("ws://localhost:80")]
    [InlineData("http://localhost:80")]
    [InlineData("//localhost:80")]
    [InlineData("/localhost")]
    [InlineData("123123123123")]
    [InlineData("           ")]
    public async Task SetWebhook_WhenUrlNotValid_ThenFailureResultReturned(string url)
    {
        var expectedError = Error.Validation(_errorWebhookCode, "Telegram:WebhookInvalid");

        var result = await _sut.SetWebhookUrlAsync(url);

        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be(expectedError.Code);
        result.Error.Description.Should().Be(expectedError.Description);
    }

    [Fact]
    public async Task SetWebhook_WhenUrlValid_ThenSuccessResultReturned()
    {
        var result = await _sut.SetWebhookUrlAsync("https://localhost:80/");

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task SetWebhook_WhenExceptionThrown_ThenFailureResultReturned()
    {
        var exception = new Exception("error");

        var expected = Error.Failure(_errorWebhookCode, exception.Message);

        _botClient.SendRequest(Arg.Any<SetWebhookRequest>()).Throws(exception);

        var result = await _sut.SetWebhookUrlAsync("https://valid.webhook");

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().BeEquivalentTo(expected);
    }
}
