using Eclipse.Application.Exporting.Reminders;
using Eclipse.Domain.Users;
using Eclipse.Infrastructure.Excel;
using Eclipse.Tests.Generators;
using Eclipse.Tests;

using NSubstitute;

using Xunit;
using FluentAssertions;

namespace Eclipse.Application.Tests.Exporting.Reminders;

public sealed class ImortRemindersStrategyTests
{
    private readonly IUserRepository _userRepository;

    private readonly ImportRemindersStrategy _sut;

    public ImortRemindersStrategyTests()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _sut = new ImportRemindersStrategy(new UserManager(_userRepository), new ExcelManager());
    }

    [Fact]
    public async Task AddReminders_WhenFileIsValid_ThenProcessedSeccessfully()
    {
        using var ms = TestsAssembly.GetValidRemindersExcelFile();

        var user = UserGenerator.Generate(1).First();

        _userRepository.FindAsync(default).ReturnsForAnyArgs(user);

        await _sut.ImportAsync(ms);

        await _userRepository.ReceivedWithAnyArgs().FindAsync(default);
        await _userRepository.Received().UpdateAsync(user);
        user.Reminders.IsNullOrEmpty().Should().BeFalse();
    }

    [Fact]
    public async Task AddReminders_WhenUserNotExist_ThenReportSend()
    {
        using var ms = TestsAssembly.GetValidRemindersExcelFile();

        _userRepository.FindAsync(default).ReturnsForAnyArgs(Task.FromResult<User?>(null));

        await _sut.ImportAsync(ms);

        await _userRepository.ReceivedWithAnyArgs().FindAsync(default);
        await _userRepository.DidNotReceiveWithAnyArgs().UpdateAsync(default!);
    }
}
