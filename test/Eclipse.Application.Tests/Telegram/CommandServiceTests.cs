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

    [Fact]
    public async Task Add_WhenCommandIsEmpty_ThenFailureResultReturned()
    {
        var expectedError = Error.Validation("Command.Add", $"{_descriptionPrefix}:CommandMinLength");

        var request = new AddCommandRequest
        {
            Command = string.Empty,
            Description = "description"
        };

        var result = await _sut.Add(request);

        result.IsSuccess.Should().BeFalse();

        var error = result.Error;
        error.Code.Should().Be(expectedError.Code);
        error.Description.Should().Be(expectedError.Description);
    }

    [Fact]
    public async Task Add_WhenCommandExcedesMaxLength_ThenFailureResultReturned()
    {
        var expectedError = Error.Validation("Command.Add", $"{_descriptionPrefix}:CommandMaxLength");

        var request = new AddCommandRequest
        {
            Command = new string('x', CommandConstants.CommandMaxLength + 1),
            Description = "description"
        };

        var result = await _sut.Add(request);

        result.IsSuccess.Should().BeFalse();

        var error = result.Error;
        error.Code.Should().Be(expectedError.Code);
        error.Description.Should().Be(expectedError.Description);
    }

    [Fact]
    public async Task Add_WhenDescriptionIsEmpty_ThenFailureResultReturned()
    {
        var expectedError = Error.Validation("Command.Add", $"{_descriptionPrefix}:DescriptionMinLength");

        var request = new AddCommandRequest
        {
            Command = "command",
            Description = string.Empty
        };

        var result = await _sut.Add(request);

        result.IsSuccess.Should().BeFalse();

        var error = result.Error;
        error.Code.Should().Be(expectedError.Code);
        error.Description.Should().Be(expectedError.Description);
    }

    [Fact]
    public async Task Add_WhenDescriptionExcedesMaxLength_ThenFailureResultReturned()
    {
        var expectedError = Error.Validation("Command.Add", $"{_descriptionPrefix}:DescriptionMaxLength");

        var request = new AddCommandRequest
        {
            Command = "command",
            Description = new string('x', CommandConstants.DescriptionMaxLength + 1)
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
