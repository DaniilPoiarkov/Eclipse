using Eclipse.Application.Contracts.Telegram;
using Eclipse.Application.Telegram;
using Eclipse.Common.Results;
using Eclipse.Tests.Builders;

using FluentAssertions;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;

using NSubstitute;

using Telegram.Bot;

using Xunit;

namespace Eclipse.Application.Tests.Telegram;

public sealed class TelegramServiceTests
{
    private readonly IStringLocalizer<TelegramService> _localizer;

    private readonly TelegramService _sut;

    private static readonly string _errorSendCode = "Telegram.Send";

    private static readonly string _errorWebhookCode = "Telegram.Webhook";

    public TelegramServiceTests()
    {
        var botClient = Substitute.For<ITelegramBotClient>();
        var configuration = Substitute.For<IConfiguration>();

        _localizer = Substitute.For<IStringLocalizer<TelegramService>>();
        _sut = new TelegramService(botClient, configuration, _localizer);
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
        LocalizerBuilder<TelegramService>.Configure(_localizer)
            .For($"Telegram:{errorCode}")
            .Return($"Error with {errorCode}");

        var expectedError = Error.Validation(_errorSendCode, _localizer[$"Telegram:{errorCode}"]);

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
        LocalizerBuilder<TelegramService>.Configure(_localizer)
            .For("Telegram:WebhookInvalid")
            .Return("Invalid webhook");

        var expectedError = Error.Validation(_errorWebhookCode, _localizer["Telegram:WebhookInvalid"]);

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
}
