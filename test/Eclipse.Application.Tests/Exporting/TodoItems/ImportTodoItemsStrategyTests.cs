using Eclipse.Application.Exporting;
using Eclipse.Application.Exporting.TodoItems;
using Eclipse.Common.Excel;
using Eclipse.Domain.Users;
using Eclipse.Tests.Generators;

using FluentAssertions;

using NSubstitute;

using System.Linq.Expressions;

using Xunit;

namespace Eclipse.Application.Tests.Exporting.TodoItems;

public sealed class ImportTodoItemsStrategyTests
{
    private readonly IExcelManager _excelManager;

    private readonly IUserRepository _userRepository;

    private readonly IImportValidator<ImportTodoItemDto, ImportTodoItemsValidationOptions> _validator;

    private readonly ImportTodoItemsStrategy _sut;

    public ImportTodoItemsStrategyTests()
    {
        _excelManager = Substitute.For<IExcelManager>();
        _userRepository = Substitute.For<IUserRepository>();
        _validator = Substitute.For<IImportValidator<ImportTodoItemDto, ImportTodoItemsValidationOptions>>();
        _sut = new ImportTodoItemsStrategy(_userRepository, _excelManager, _validator);
    }

    [Fact]
    public void Type_WhenChecked_ThenReturnsTodoItemSpecification()
    {
        _sut.Type.Should().Be(ImportType.TodoItems);
    }

    [Fact]
    public async Task ImportAsync_WhenRowCannotBeImported_ThenReturnedWithinFailedResult()
    {
        var user = UserGenerator.Get();

        for (int i = 0; i < 6; i++)
        {
            user.AddTodoItem($"Todo item #{i + 1}", DateTime.UtcNow.AddMinutes(-i));
        }

        var todoItem1 = ImportEntityRowGenerator.TodoItem();
        var todoItem2 = ImportEntityRowGenerator.TodoItem();
        var todoItem3 = ImportEntityRowGenerator.TodoItem();

        todoItem1.Exception = "exception";

        todoItem1.UserId = user.Id;
        todoItem2.UserId = user.Id;
        todoItem3.UserId = user.Id;

        using var ms = new MemoryStream();

        _excelManager.Read<ImportTodoItemDto>(ms).Returns([todoItem1, todoItem2, todoItem3]);

        _validator.ValidateAndSetErrors(
            Arg.Any<IEnumerable<ImportTodoItemDto>>()
        ).Returns([todoItem1, todoItem2, todoItem3]);

        _userRepository.GetByExpressionAsync(
            Arg.Any<Expression<Func<User, bool>>>()
        ).Returns([user]);

        var result = await _sut.ImportAsync(ms);

        result.IsSuccess.Should().BeFalse();
        result.FailedRows.Count.Should().Be(2);
        result.FailedRows[1].Exception.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task ImportAsync_WhenRowsAreValid_ThenProcessedSuccessfully()
    {
        using var stream = new MemoryStream();

        IEnumerable<ImportTodoItemDto> rows = [
            ImportEntityRowGenerator.TodoItem(),
            ImportEntityRowGenerator.TodoItem(),
        ];

        var users = UserGenerator.GetWithIds(rows.Select(r => r.UserId)).ToList();

        _excelManager.Read<ImportTodoItemDto>(stream).Returns(rows);
        _userRepository.GetByExpressionAsync(_ => true).ReturnsForAnyArgs(users);
        _validator.ValidateAndSetErrors(rows).ReturnsForAnyArgs(rows);

        var result = await _sut.ImportAsync(stream);

        result.IsSuccess.Should().BeTrue();
        result.FailedRows.Should().BeEmpty();

        await _userRepository.ReceivedWithAnyArgs().GetByExpressionAsync(_ => true);

        foreach (var user in users)
        {
            await _userRepository.Received().UpdateAsync(user);
            user.TodoItems.Should().HaveCount(1);
        }
    }

    [Fact]
    public async Task ImportAsync_WhenUserNotExist_ThenFailureResultReturned()
    {
        using var stream = new MemoryStream();

        var row = ImportEntityRowGenerator.TodoItem();

        _userRepository.GetByExpressionAsync(_ => true).ReturnsForAnyArgs([]);
        _excelManager.Read<ImportTodoItemDto>(stream).Returns([row]);
        _validator.ValidateAndSetErrors([row]).ReturnsForAnyArgs([row]);

        await _sut.ImportAsync(stream);

        await _userRepository.ReceivedWithAnyArgs().GetByExpressionAsync(_ => true);
        await _userRepository.DidNotReceiveWithAnyArgs().UpdateAsync(default!);
    }
}
