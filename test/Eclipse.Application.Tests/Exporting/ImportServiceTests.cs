using Eclipse.Application.Contracts.Exporting;
using Eclipse.Application.Exporting;

using NSubstitute;

using Xunit;

namespace Eclipse.Application.Tests.Exporting;

public sealed class ImportServiceTests
{
    private readonly ImportService _sut;

    private readonly IImportStrategy _usersStrategy;

    private readonly IImportStrategy _todoItemsStrategy;

    private readonly IImportStrategy _remindersStrategy;

    public ImportServiceTests()
    {
        _usersStrategy = Substitute.For<IImportStrategy>();
        _usersStrategy.Type.Returns(ImportType.Users);

        _todoItemsStrategy = Substitute.For<IImportStrategy>();
        _todoItemsStrategy.Type.Returns(ImportType.TodoItems);

        _remindersStrategy = Substitute.For<IImportStrategy>();
        _remindersStrategy.Type.Returns(ImportType.Reminders);

        _sut = new ImportService([_usersStrategy, _todoItemsStrategy, _remindersStrategy]);
    }

    [Fact]
    public async Task AddUsersAsync_WhenCalled_ThenDelegatedToProperStrategy()
    {
        _usersStrategy.ImportAsync(default!).ReturnsForAnyArgs(new ImportResult<ImportEntityBase>());

        await _sut.AddUsersAsync(default!);

        await _usersStrategy.ReceivedWithAnyArgs().ImportAsync(default!);
        await _todoItemsStrategy.DidNotReceiveWithAnyArgs().ImportAsync(default!);
        await _remindersStrategy.DidNotReceiveWithAnyArgs().ImportAsync(default!);
    }

    [Fact]
    public async Task AddTodoItemsAsync_WhenCalled_ThenDelegatedToProperStrategy()
    {
        _usersStrategy.ImportAsync(default!).ReturnsForAnyArgs(new ImportResult<ImportEntityBase>());

        await _sut.AddTodoItemsAsync(default!);

        await _todoItemsStrategy.ReceivedWithAnyArgs().ImportAsync(default!);
        await _usersStrategy.DidNotReceiveWithAnyArgs().ImportAsync(default!);
        await _remindersStrategy.DidNotReceiveWithAnyArgs().ImportAsync(default!);
    }

    [Fact]
    public async Task AddRemindersAsync_WhenCalled_ThenDelegatedToProperStrategy()
    {
        _usersStrategy.ImportAsync(default!).ReturnsForAnyArgs(new ImportResult<ImportEntityBase>());

        await _sut.AddRemindersAsync(default!);

        await _remindersStrategy.ReceivedWithAnyArgs().ImportAsync(default!);
        await _usersStrategy.DidNotReceiveWithAnyArgs().ImportAsync(default!);
        await _todoItemsStrategy.DidNotReceiveWithAnyArgs().ImportAsync(default!);
    }
}
