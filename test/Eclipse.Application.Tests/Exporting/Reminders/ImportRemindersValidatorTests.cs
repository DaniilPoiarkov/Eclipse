﻿using Eclipse.Application.Exporting.Reminders;
using Eclipse.Domain.Reminders;
using Eclipse.Domain.Users;
using Eclipse.Tests.Builders;
using Eclipse.Tests.Extensions;
using Eclipse.Tests.Generators;

using FluentAssertions;

using Microsoft.Extensions.Localization;

using NSubstitute;

using Xunit;

namespace Eclipse.Application.Tests.Exporting.Reminders;

public sealed class ImportRemindersValidatorTests
{
    private readonly IStringLocalizer<ImportRemindersValidator> _localizer;

    private readonly ImportRemindersValidator _sut;

    public ImportRemindersValidatorTests()
    {
        _localizer = Substitute.For<IStringLocalizer<ImportRemindersValidator>>();
        _sut = new(_localizer);
    }

    [Fact]
    public void ValidateAndSetErrors_WhenValidRecordsPassed_ThenNoErrorSet()
    {
        IEnumerable<ImportReminderDto> rows = [
            ImportEntityRowGenerator.Reminder(),
            ImportEntityRowGenerator.Reminder()
        ];

        var users = UserGenerator.GetWithIds(rows.Select(r => r.UserId)).ToList();

        var options = new ImportRemindersValidationOptions
        {
            Users = users
        };

        _sut.Set(options);

        var result = _sut.ValidateAndSetErrors(rows);

        foreach (var row in result)
        {
            row.Exception.Should().BeEmpty();
        }
    }

    [Fact]
    public void ValidateAndSetErrors_WhenOptionsNotPassed_ThenNoExceptionsThrowed()
    {
        IEnumerable<ImportReminderDto> rows = [
            ImportEntityRowGenerator.Reminder(),
            ImportEntityRowGenerator.Reminder()
        ];

        var action = () => _sut.ValidateAndSetErrors(rows);

        action.Should().NotThrow();
    }

    [Fact]
    public void ValidateAndSetErrors_WhenUserNotFound_ThenErrorSet()
    {
        var error = $"{nameof(User)} not found";

        var localizer = LocalizerBuilder<ImportRemindersValidator>.Create()
            .ForWithArgs("{0}NotFound", nameof(User))
            .Return(error)
            .Build();

        _localizer.DelegateCalls(localizer);

        var result = _sut.ValidateAndSetErrors([ImportEntityRowGenerator.Reminder()]);

        foreach (var row in result)
        {
            row.Exception.Should().Be(error);
        }
    }

    [Fact]
    public void ValidateAndSetErrors_WhenMultipleErrorsPresent_ThenCombinedAndErrorSet()
    {
        var invalidRow = ImportEntityRowGenerator.Reminder("qwerty");

        var notFoundError = $"{nameof(User)} not found";
        var fieldError = $"Invalid field {nameof(invalidRow.NotifyAt)} \'{invalidRow.NotifyAt}\'";

        var localizer = LocalizerBuilder<ImportRemindersValidator>.Create()
            .ForWithArgs("{0}NotFound", nameof(User))
                .Return(notFoundError)
            .ForWithArgs("InvalidField{0}{1}", nameof(invalidRow.NotifyAt), invalidRow.NotifyAt)
                .Return(fieldError)
            .Build();

        _localizer.DelegateCalls(localizer);

        string[] expectedErrors = [
            fieldError,
            notFoundError
        ];

        var result = _sut.ValidateAndSetErrors([invalidRow]);

        foreach (var row in result)
        {
            row.Exception?.Split(", ").Should().BeEquivalentTo(expectedErrors);
        }
    }

    [Fact]
    public void ValidateAndSetErrors_WhenReminderAlreadyExists_ShouldReturnError()
    {
        var user = UserGenerator.Get();
        var reminder = user.AddReminder("test", TimeOnly.FromDateTime(DateTime.UtcNow));

        var reminders = new List<ImportReminderDto>
        {
            new() { UserId = user.Id, Id = reminder.Id, NotifyAt = "08:00" }
        };

        var options = new ImportRemindersValidationOptions
        {
            Users = [user]
        };

        var expectedError = $"{nameof(Reminder)} with {nameof(reminder.Id)} \'{reminders[0].Id}\' already exists";

        var localizer = LocalizerBuilder<ImportRemindersValidator>.Create()
            .ForWithArgs("{0}AlreadyExists{1}{2}", nameof(Reminder), nameof(reminder.Id), reminders[0].Id)
            .Return(expectedError)
            .Build();

        _localizer.DelegateCalls(localizer);

        _sut.Set(options);

        var result = _sut.ValidateAndSetErrors(reminders).ToList();

        result.Should().HaveCount(1);
        result[0].Exception.Should().Contain(expectedError);
    }

    [Fact]
    public void ValidateAndSetErrors_WhenInvalidNotifyAt_ShouldReturnError()
    {
        var user = UserGenerator.Get();

        var row = new ImportReminderDto { UserId = user.Id, Id = Guid.NewGuid(), NotifyAt = "invalid_time" };

        var options = new ImportRemindersValidationOptions
        {
            Users = [user]
        };

        var expectedError = $"Invalid field {nameof(row.NotifyAt)} \'{row.NotifyAt}\'";

        var localizer = LocalizerBuilder<ImportRemindersValidator>.Create()
            .ForWithArgs("InvalidField{0}{1}", nameof(row.NotifyAt), row.NotifyAt)
            .Return(expectedError)
            .Build();

        _localizer.DelegateCalls(localizer);

        _sut.Set(options);

        var result = _sut.ValidateAndSetErrors([row]).ToList();

        result.Should().HaveCount(1);
        result[0].Exception.Should().Contain(expectedError);
    }
}
