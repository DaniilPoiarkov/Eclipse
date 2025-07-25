﻿using Eclipse.Application.Exporting.Users;
using Eclipse.Domain.Users;
using Eclipse.Tests.Builders;
using Eclipse.Tests.Extensions;
using Eclipse.Tests.Generators;

using FluentAssertions;

using Microsoft.Extensions.Localization;

using NSubstitute;

using Xunit;

namespace Eclipse.Application.Tests.Exporting.Users;

public sealed class ImportUsersValidatorTests
{
    private readonly IStringLocalizer<ImportUsersValidator> _localizer;

    private readonly ImportUsersValidator _sut;

    public ImportUsersValidatorTests()
    {
        _localizer = Substitute.For<IStringLocalizer<ImportUsersValidator>>();
        _sut = new ImportUsersValidator(_localizer);
    }

    [Fact]
    public void ValidateAndSetErrors_WhenGmtIsInvalid_ThenErrorSet()
    {
        var row = ImportEntityRowGenerator.User(gmt: "qwerty");

        var error = $"Invalid field {nameof(row.Gmt)}\'{row.Gmt}\'";

        var localizer = LocalizerBuilder<ImportUsersValidator>.Create()
            .ForWithArgs("InvalidField{0}{1}", nameof(row.Gmt), row.Gmt)
                .Return(error)
            .Build();

        _localizer.DelegateCalls(localizer);

        var result = _sut.ValidateAndSetErrors([row]).Single();
        result.Exception.Should().Be(error);
    }

    [Fact]
    public void ValidateAndSetErrors_ShouldReturnError_WhenUserAlreadyExists()
    {
        // Arrange
        var user = UserGenerator.Get(123);

        var rows = new List<ImportUserDto>
        {
            new() { Id = user.Id, UserName = "new_user", ChatId = 456 },
            new() { Id = Guid.NewGuid(), UserName = user.UserName, ChatId = 789 },
            new() { Id = Guid.NewGuid(), UserName = "another_user", ChatId = user.ChatId }
        };

        var options = new ImportUsersValidationOptions { Users = [user] };
        _sut.Set(options);

        var template = "{0}AlreadyExists{1}{2}";
        var idError = $"{nameof(User)} already exists with {nameof(ImportUserDto.Id)} \'{rows[0].Id}\'";
        var userNameError = $"{nameof(User)} already exists with {nameof(ImportUserDto.UserName)} \'{rows[1].UserName}\'";
        var chatIdError = $"{nameof(User)} already exists with {nameof(ImportUserDto.ChatId)} \'{rows[2].ChatId}\'";

        var localizer = LocalizerBuilder<ImportUsersValidator>.Create()
            .ForWithArgs(template, nameof(User), nameof(ImportUserDto.Id), rows[0].Id)
                .Return(idError)
            .ForWithArgs(template, nameof(User), nameof(ImportUserDto.UserName), rows[1].UserName)
                .Return(userNameError)
            .ForWithArgs(template, nameof(User), nameof(ImportUserDto.ChatId), rows[2].ChatId)
                .Return(chatIdError)
            .Build();

        _localizer.DelegateCalls(localizer);

        // Act
        var result = _sut.ValidateAndSetErrors(rows).ToList();

        // Assert
        result[0].Exception.Should().Be(idError);
        result[1].Exception.Should().Be(userNameError);
        result[2].Exception.Should().Be(chatIdError);
    }
}
