using Eclipse.Application.Exporting.TodoItems;
using Eclipse.Domain.Users;
using Eclipse.Infrastructure.Excel;
using Eclipse.Tests;
using Eclipse.Tests.Generators;

using FluentAssertions;

using NSubstitute;

using Xunit;

namespace Eclipse.Application.Tests.Exporting.TodoItems;

public sealed class ImportTodoItemsStrategyTests
{
    private readonly IUserRepository _userRepository;

    private readonly ImportTodoItemsStrategy _sut;

    public ImportTodoItemsStrategyTests()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _sut = new ImportTodoItemsStrategy(new UserManager(_userRepository), new ExcelManager());
    }

    [Fact]
    public async Task AddTodoItems_WhenFileIsValid_ThenProcessedSeccessfully()
    {
        using var stream = TestsAssembly.GetValidTodoItemsExcelFile();

        var user = UserGenerator.Generate(1).First();

        _userRepository.FindAsync(default).ReturnsForAnyArgs(user);

        await _sut.ImportAsync(stream);

        await _userRepository.ReceivedWithAnyArgs().FindAsync(default);
        await _userRepository.Received().UpdateAsync(user);
        user.TodoItems.IsNullOrEmpty().Should().BeFalse();
    }

    [Fact]
    public async Task AddTodoItems_WhenUserNotExist_ThenReportSend()
    {
        using var stream = TestsAssembly.GetValidTodoItemsExcelFile();

        _userRepository.FindAsync(default).ReturnsForAnyArgs(Task.FromResult<User?>(null));

        await _sut.ImportAsync(stream);

        await _userRepository.ReceivedWithAnyArgs().FindAsync(default);
        await _userRepository.DidNotReceiveWithAnyArgs().UpdateAsync(default!);
    }
}
