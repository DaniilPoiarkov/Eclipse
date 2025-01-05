using Eclipse.Application.Contracts.Telegram.Commands;
using Eclipse.Application.Telegram.Commands;
using Eclipse.Common.Results;

using FluentAssertions;

using NSubstitute;

using Telegram.Bot;
using Telegram.Bot.Requests;
using Telegram.Bot.Types;

using Xunit;

namespace Eclipse.Application.Tests.Telegram;

public sealed class CommandServiceTests
{
    private readonly CommandService _sut;

    private readonly ITelegramBotClient _botClient;

    private static readonly string _descriptionPrefix = "BotCommand";

    public CommandServiceTests()
    {
        _botClient = Substitute.For<ITelegramBotClient>();
        _sut = new CommandService(_botClient);
    }

    [Theory]
    [InlineData("", "description", "CommandMinLength")]
    [InlineData("commandcommandcommandcommandcommand", "description", "CommandMaxLength")]
    [InlineData("     ", "description", "CommandInvalidFormat")]
    [InlineData("command.1", "description", "CommandInvalidFormat")]
    [InlineData("command?1", "description", "CommandInvalidFormat")]
    [InlineData("command!1", "description", "CommandInvalidFormat")]
    [InlineData("command-1", "description", "CommandInvalidFormat")]
    [InlineData("command", "", "DescriptionMinLength")]
    [InlineData("command", "d", "DescriptionMinLength")]
    [InlineData("command", "de", "DescriptionMinLength")]
    [InlineData("command", "     ", "DescriptionMinLength")]
    [InlineData("command",
        "DescriptionMaxLengthDescriptionMaxLengthDescriptionMaxLengthDescriptionMaxLengthDescriptionMaxLengthDescriptionMaxLengthDescriptionMaxLengthDescriptionMaxLength" +
        "DescriptionMaxLengthDescriptionMaxLengthDescriptionMaxLengthDescriptionMaxLengthDescriptionMaxLength",
        "DescriptionMaxLength")]
    public async Task Add_WhenRequestInvalid_ThenProperFailureResultReturned(string command, string description, string errorCode)
    {
        var expectedError = Error.Validation("Command.Add", $"{_descriptionPrefix}:{errorCode}");

        var request = new AddCommandRequest
        {
            Command = command,
            Description = description
        };

        var result = await _sut.Add(request);

        result.IsSuccess.Should().BeFalse();

        result.Error.Should().BeEquivalentTo(expectedError);
    }

    [Theory]
    [InlineData("command", "proper description")]
    [InlineData("command_new", "description without meaning")]
    [InlineData("command_1", "description with specific meaning")]
    [InlineData("123_command", "description \n\t123!@#")]
    [InlineData("123_123", "description_!@#")]
    [InlineData("123123", "description 123")]
    public async Task Add_WhenRequestIsValid_ThenSuccessResultReturned(string command, string description)
    {
        var request = new AddCommandRequest
        {
            Command = command,
            Description = description
        };

        var result = await _sut.Add(request);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task GetList_WhenCalled_ThenAvailableCommandsReturned()
    {
        var command = new BotCommand()
        {
            Command = "/test",
            Description = "Test"
        };

        _botClient.SendRequest(Arg.Any<GetMyCommandsRequest>()).Returns([command]);

        var result = await _sut.GetList();

        result.Count.Should().Be(1);
        result[0].Command.Should().Be(command.Command);
        result[0].Description.Should().Be(command.Description);
    }
}
