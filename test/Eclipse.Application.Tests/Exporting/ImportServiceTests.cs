using Eclipse.Application.Contracts.Exporting;
using Eclipse.Application.Exporting;
using Eclipse.Common.Excel;
using Eclipse.Domain.Users;
using Eclipse.Infrastructure.Excel;
using Eclipse.Tests;
using Eclipse.Tests.Generators;

using FluentAssertions;

using NSubstitute;

using Xunit;

namespace Eclipse.Application.Tests.Exporting;

public sealed class ImportServiceTests
{
    private readonly IUserRepository _userRepository;

    private readonly IExcelManager _excelManager = new ExcelManager();

    private readonly Lazy<IImportService> _sut;

    private IImportService Sut => _sut.Value;

    public ImportServiceTests()
    {
        _userRepository = Substitute.For<IUserRepository>();

        _sut = new(() => new ImportService(
            new UserManager(_userRepository),
            _excelManager)
        );
    }

    [Fact]
    public async Task AddUsers_WhenFileIsValid_ThenProcessedSuccessfully()
    {
        using var stream = TestsAssembly.GetValidUsersExcelFile();

        await Sut.AddUsersAsync(stream);

        await _userRepository.ReceivedWithAnyArgs().CreateAsync(default!);
    }

    [Fact]
    public async Task AddUsers_WhenRecordsInvalid_ThenReportSend()
    {
        using var stream = TestsAssembly.GetInvalidUsersExcelFile();

        await Sut.AddUsersAsync(stream);

        await _userRepository.DidNotReceiveWithAnyArgs().CreateAsync(default!);
    }

    [Fact]
    public async Task AddTodoItems_WhenFileIsValid_ThenProcessedSeccessfully()
    {
        using var stream = TestsAssembly.GetValidTodoItemsExcelFile();

        var user = UserGenerator.Generate(1).First();

        _userRepository.FindAsync(default).ReturnsForAnyArgs(user);

        await Sut.AddTodoItemsAsync(stream);

        await _userRepository.ReceivedWithAnyArgs().FindAsync(default);
        await _userRepository.Received().UpdateAsync(user);
        user.TodoItems.IsNullOrEmpty().Should().BeFalse();
    }

    [Fact]
    public async Task AddTodoItems_WhenUserNotExist_ThenReportSend()
    {
        using var stream = TestsAssembly.GetValidTodoItemsExcelFile();

        _userRepository.FindAsync(default).ReturnsForAnyArgs(Task.FromResult<User?>(null));

        await Sut.AddTodoItemsAsync(stream);

        await _userRepository.ReceivedWithAnyArgs().FindAsync(default);
        await _userRepository.DidNotReceiveWithAnyArgs().UpdateAsync(default!);
    }

    [Fact]
    public async Task AddReminders_WhenFileIsValid_ThenProcessedSeccessfully()
    {
        using var ms = TestsAssembly.GetValidRemindersExcelFile();

        var user = UserGenerator.Generate(1).First();

        _userRepository.FindAsync(default).ReturnsForAnyArgs(user);

        await Sut.AddRemindersAsync(ms);

        await _userRepository.ReceivedWithAnyArgs().FindAsync(default);
        await _userRepository.Received().UpdateAsync(user);
        user.Reminders.IsNullOrEmpty().Should().BeFalse();
    }

    [Fact]
    public async Task AddReminders_WhenUserNotExist_ThenReportSend()
    {
        using var ms = TestsAssembly.GetValidRemindersExcelFile();

        _userRepository.FindAsync(default).ReturnsForAnyArgs(Task.FromResult<User?>(null));

        await Sut.AddRemindersAsync(ms);

        await _userRepository.ReceivedWithAnyArgs().FindAsync(default);
        await _userRepository.DidNotReceiveWithAnyArgs().UpdateAsync(default!);
    }
}
