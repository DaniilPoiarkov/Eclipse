using Bogus;

using Eclipse.Application.Exporting.Reminders;
using Eclipse.Domain.Users;
using Eclipse.Tests.Generators;
using Eclipse.Tests.Utils;

using FluentAssertions;

using Microsoft.Extensions.Localization;

using Xunit;

namespace Eclipse.Application.Tests.Exporting.Reminders;

public sealed class ImportRemindersValidatorTests
{
    private readonly IStringLocalizer<ImportRemindersValidator> _localizer;

    private readonly ImportRemindersValidator _sut;

    private static readonly Dictionary<string, string> _localizations = new()
    {
        ["{0}NotFound"] = "{0} not found",
        ["{0}AlreadyExists{1}{2}"] = "{0} with {1} \'{2}\' already exists",
        ["InvalidField{0}{1}"] = "Invalid field {0} \'{1}\'"
    };

    public ImportRemindersValidatorTests()
    {
        _localizer = new EmptyStringLocalizer<ImportRemindersValidator>(_localizations);
        _sut = new(_localizer);
    }

    [Fact]
    public void ValidateAndSetErrors_WhenValidRecordsPassed_ThenNoErrorSet()
    {
        IEnumerable<ImportReminderDto> rows = [
            GetRow(),
            GetRow()
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
            GetRow(),
            GetRow()
        ];

        var action = () => _sut.ValidateAndSetErrors(rows);

        action.Should().NotThrow();
    }

    [Fact]
    public void ValidateAndSetErrors_WhenUserNotFound_ThenErrorSet()
    {
        var expectedError = _localizer["{0}NotFound", nameof(User)];

        var result = _sut.ValidateAndSetErrors([GetRow()]);

        foreach (var row in result)
        {
            row.Exception.Should().Be(expectedError);
        }
    }

    [Fact]
    public void ValidateAndSetErrors_WhenMultipleErrorsPresent_ThenCombinedAndErrorSet()
    {
        var invalidRow = GetRow("qwerty");

        string[] expectedErrors = [
            _localizer["InvalidField{0}{1}", nameof(invalidRow.NotifyAt), invalidRow.NotifyAt],
            _localizer["{0}NotFound", nameof(User)]
        ];

        var result = _sut.ValidateAndSetErrors([invalidRow]);

        foreach (var row in result)
        {
            row.Exception?.Split(", ").Should().BeEquivalentTo(expectedErrors);
        }
    }

    private static ImportReminderDto GetRow(string notifyAt = "12:00:00")
    {
        var faker = new Faker();

        return new ImportReminderDto
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            NotifyAt = notifyAt,
            Text = faker.Lorem.Word()
        };
    }
}
