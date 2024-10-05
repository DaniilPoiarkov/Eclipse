using Eclipse.Application.Exporting;
using Eclipse.Application.Exporting.Reminders;
using Eclipse.Common.Excel;
using Eclipse.Domain.Users;
using Eclipse.Tests.Generators;

using FluentAssertions;

using NSubstitute;

using Xunit;

namespace Eclipse.Application.Tests.Exporting.Reminders;

public sealed class ImortRemindersStrategyTests
{
    private readonly IExcelManager _excelManager;

    private readonly IUserRepository _userRepository;

    private readonly IImportValidator<ImportReminderDto, ImportRemindersValidationOptions> _validator;

    private readonly ImportRemindersStrategy _sut;

    public ImortRemindersStrategyTests()
    {
        _excelManager = Substitute.For<IExcelManager>();
        _userRepository = Substitute.For<IUserRepository>();
        _validator = Substitute.For<IImportValidator<ImportReminderDto, ImportRemindersValidationOptions>>();
        _sut = new ImportRemindersStrategy(_userRepository, _excelManager, _validator);
    }

    [Fact]
    public void Type_WhenChecked_ThenReturnsRemindersSpecification()
    {
        _sut.Type.Should().Be(ImportType.Reminders);
    }

    [Fact]
    public async Task ImportAsync_WhenRowCannotBeImported_ThenReturnedWithinFailedResult()
    {
        var reminder = ImportEntityRowGenerator.Reminder();
        reminder.Exception = "exception";

        using var ms = new MemoryStream();

        _excelManager.Read<ImportReminderDto>(ms).Returns([reminder]);
        _validator.ValidateAndSetErrors(
            Arg.Any<IEnumerable<ImportReminderDto>>()
        ).Returns([reminder]);

        var result = await _sut.ImportAsync(ms);

        await _userRepository.DidNotReceiveWithAnyArgs().UpdateAsync(Arg.Any<User>());
        result.IsSuccess.Should().BeFalse();
        result.FailedRows.Count.Should().Be(1);
        result.FailedRows[0].Should().Be(reminder);
    }

    [Fact]
    public async Task ImportAsync_WhenRecordsValid_ThenImportedSuccessfully()
    {
        using var ms = new MemoryStream();

        IEnumerable<ImportReminderDto> rows = [
            ImportEntityRowGenerator.Reminder(),
            ImportEntityRowGenerator.Reminder(),
        ];

        var users = UserGenerator.GetWithIds(rows.Select(r => r.UserId)).ToList();

        _excelManager.Read<ImportReminderDto>(ms).Returns(rows);
        _validator.ValidateAndSetErrors(rows).ReturnsForAnyArgs(rows);
        _userRepository.GetByExpressionAsync(_ => true).ReturnsForAnyArgs(users);

        var result = await _sut.ImportAsync(ms);

        result.IsSuccess.Should().BeTrue();
        result.FailedRows.Should().BeEmpty();

        _validator.ReceivedWithAnyArgs().Set(new());

        foreach (var user in users)
        {
            user.Reminders.Should().HaveCount(1);
            await _userRepository.Received().UpdateAsync(user);
        }
    }

    [Fact]
    public async Task ImportAsync_WhenUserNotExist_ThenFailedResultReturned()
    {
        using var ms = new MemoryStream();

        var row = ImportEntityRowGenerator.Reminder();

        _excelManager.Read<ImportReminderDto>(ms).Returns([row]);
        _userRepository.GetByExpressionAsync(_ => true).ReturnsForAnyArgs([]);
        _validator.ValidateAndSetErrors([row]).ReturnsForAnyArgs([row]);

        var result = await _sut.ImportAsync(ms);

        result.IsSuccess.Should().BeFalse();
        result.FailedRows.Should().HaveCount(1);

        await _userRepository.DidNotReceiveWithAnyArgs().UpdateAsync(default!);
    }
}
