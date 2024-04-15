using Eclipse.Application.Contracts.Telegram.Commands;
using Eclipse.Application.Telegram.Commands;
using Eclipse.Common.Results;

using FluentAssertions;

using NSubstitute;

using Telegram.Bot;

using Xunit;

namespace Eclipse.Application.Tests.Telegram;

public sealed class CommandServiceTests
{    
    private readonly ICommandService _sut;

    private static readonly string _descriptionPrefix = "BotCommand";

    public CommandServiceTests()
    {
        var botClient = Substitute.For<ITelegramBotClient>();
        _sut = new CommandService(botClient);
    }

    [Theory]
    [InlineData("", "description", "CommandMinLength")]
    [InlineData("commandcommandcommandcommandcommand", "description", "CommandMaxLength")]
    [InlineData("command", "", "DescriptionMinLength")]
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

        var error = result.Error;
        error.Code.Should().Be(expectedError.Code);
        error.Description.Should().Be(expectedError.Description);
    }

    
    [Fact]
    public async Task Add_WhenRequestIsValid_ThenSuccessResultReturned()
    {
        var request = new AddCommandRequest
        {
            Command = "command",
            Description = "description"
        };

        var result = await _sut.Add(request);

        result.IsSuccess.Should().BeTrue();
    }
}
