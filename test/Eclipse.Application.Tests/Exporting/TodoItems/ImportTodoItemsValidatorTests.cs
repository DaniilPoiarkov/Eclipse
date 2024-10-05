using Eclipse.Application.Exporting.TodoItems;
using Eclipse.Domain.Shared.TodoItems;
using Eclipse.Domain.TodoItems;
using Eclipse.Domain.Users;
using Eclipse.Tests.Builders;
using Eclipse.Tests.Extensions;
using Eclipse.Tests.Generators;

using FluentAssertions;

using Microsoft.Extensions.Localization;

using NSubstitute;

using Xunit;

namespace Eclipse.Application.Tests.Exporting.TodoItems;

public sealed class ImportTodoItemsValidatorTests
{
    private readonly IStringLocalizer<ImportTodoItemsValidator> _localizer;

    private readonly ImportTodoItemsValidator _sut;

    public ImportTodoItemsValidatorTests()
    {
        _localizer = Substitute.For<IStringLocalizer<ImportTodoItemsValidator>>();
        _sut = new(_localizer);
    }

    [Fact]
    public void ValidateAndSetErrors_WhenRowInvalid_ThenExceptionSet()
    {
        var user = UserGenerator.Get();

        for (int i = 0; i < 6; i++)
        {
            user.AddTodoItem($"Todo item #{i + 1}");
        }

        var todoItem1 = ImportEntityRowGenerator.TodoItem();
        var todoItem2 = ImportEntityRowGenerator.TodoItem();
        var todoItem3 = ImportEntityRowGenerator.TodoItem();

        todoItem1.Text = "";
        todoItem2.Id = user.TodoItems.First().Id;

        todoItem1.UserId = user.Id;
        todoItem2.UserId = user.Id;
        todoItem3.UserId = user.Id;
        
        var options = new ImportTodoItemsValidationOptions
        {
            Users = [user]
        };

        _sut.Set(options);

        LocalizerBuilder<ImportTodoItemsValidator>.Configure(_localizer)
            .For("TodoItem:Empty")
                .Return("Empty")
            .ForWithArgs("{0}AlreadyExists{1}{2}", nameof(TodoItem), nameof(todoItem2.Id), todoItem2.Id)
                .Return($"Todo item with Id {todoItem2.Id} already exists.");

        var result = _sut.ValidateAndSetErrors([ todoItem1, todoItem2, todoItem3 ]).ToList();

        result[0].Exception.Should().Be("Empty");
        result[1].Exception.Should().Be($"Todo item with Id {todoItem2.Id} already exists.");
        result[2].Exception.Should().BeNullOrEmpty();
    }

    [Fact]
    public void ValidateAndSetErrors_WhenUserReachedLimit_ThenExceptionSet()
    {
        var user = UserGenerator.Get();

        for (int i = 0; i < 7; i++)
        {
            user.AddTodoItem($"Todo item #{i + 1}");
        }

        var todoItem = ImportEntityRowGenerator.TodoItem();

        todoItem.UserId = user.Id;

        var options = new ImportTodoItemsValidationOptions
        {
            Users = [user]
        };

        _sut.Set(options);

        LocalizerBuilder<ImportTodoItemsValidator>.Configure(_localizer)
            .ForWithArgs("TodoItem:Limit", TodoItemConstants.Limit)
                .Return($"Limit - {TodoItemConstants.Limit}");

        var result = _sut.ValidateAndSetErrors([todoItem]).ToList();

        result[0].Exception.Should().Be($"Limit - {TodoItemConstants.Limit}");
    }

    [Fact]
    public void ValidateAndSetErrorsAsync_WhenRecordsValid_ThenNoErrorsSet()
    {
        IEnumerable<ImportTodoItemDto> rows = [
            ImportEntityRowGenerator.TodoItem(),
            ImportEntityRowGenerator.TodoItem(),
        ];

        var users = UserGenerator.GetWithIds(rows.Select(r => r.UserId)).ToList();

        var options = new ImportTodoItemsValidationOptions
        {
            Users = users,
        };

        _sut.Set(options);

        var result = _sut.ValidateAndSetErrors(rows);

        foreach (var row in result)
        {
            row.Exception.Should().BeEmpty();
        }
    }

    [Fact]
    public void ValidateAndSetErrors_WhenMultipleErrorsPresent_ThenCombinedErrorSet()
    {
        var invalidRow = new ImportTodoItemDto
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            Text = new string('x', TodoItemConstants.MaxLength + 1)
        };

        var notFoundError = $"{nameof(User)} not found";
        var maxLengthError = $"Max length exceded: {TodoItemConstants.MaxLength}";

        var localizer = LocalizerBuilder<ImportTodoItemsValidator>.Create()
            .ForWithArgs("{0}NotFound", nameof(User))
                .Return(notFoundError)
            .ForWithArgs("TodoItem:MaxLength", TodoItemConstants.MaxLength)
                .Return(maxLengthError)
            .Build();

        _localizer.DelegateCalls(localizer);

        string[] expectedErrors = [
            notFoundError,
            maxLengthError
        ];

        var result = _sut.ValidateAndSetErrors([invalidRow]);

        foreach (var row in result)
        {
            row.Exception?.Split(", ").Should().BeEquivalentTo(expectedErrors);
        }
    }
}
