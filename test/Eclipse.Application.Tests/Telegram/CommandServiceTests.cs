﻿using Eclipse.Application.Contracts.Telegram.Commands;
using Eclipse.Application.Telegram.Commands;
using Eclipse.Common.Results;
using Eclipse.Tests.Builders;

using FluentAssertions;

using Microsoft.Extensions.Localization;

using NSubstitute;

using Telegram.Bot;

using Xunit;

namespace Eclipse.Application.Tests.Telegram;

public sealed class CommandServiceTests
{
    private readonly IStringLocalizer<CommandService> _localizer;

    private readonly CommandService _sut;

    private static readonly string _descriptionPrefix = "BotCommand";

    public CommandServiceTests()
    {
        var botClient = Substitute.For<ITelegramBotClient>();
        _localizer = Substitute.For<IStringLocalizer<CommandService>>();
        _sut = new CommandService(botClient, _localizer);
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
        LocalizerBuilder<CommandService>.Configure(_localizer)
            .For($"{_descriptionPrefix}:{errorCode}")
            .Return($"Error fot {errorCode}");

        var expectedError = Error.Validation("Command.Add", _localizer[$"{_descriptionPrefix}:{errorCode}"]);

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
}
