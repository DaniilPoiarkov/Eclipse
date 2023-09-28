using Eclipse.Application.Contracts.TodoItems;
using Eclipse.Application.TodoItems;
using Eclipse.Application.TodoItems.Exceptions;
using Eclipse.Domain.TodoItems;
using Eclipse.Tests.Builders;

using FluentAssertions;

using NSubstitute;

using Xunit;

namespace Eclipse.Application.Tests.TodoItems;

public class TodoItemsServiceTests
{
    [Fact]
    public async Task CreateAsync_WhenInputValid_ThenCreatedSuccessfully()
    {
        var repository = Substitute.For<ITodoItemRepository>();
        repository.GetByExpressionAsync(i => i.TelegramUserId == 1).ReturnsForAnyArgs(new List<TodoItem>());

        var sut = CreateSut(repository);

        var createModel = new CreateTodoItemDto
        {
            Text = "text",
            UserId = 1,
        };

        var result = await sut.CreateAsync(createModel);

        result.Should().NotBeNull();
        result.Text.Should().Be(createModel.Text);
        result.TelegramUserId.Should().Be(createModel.UserId);
        result.Id.Should().NotBeEmpty();
    }

    [Fact]
    public async Task CreateAsync_WhenUserReachLimitOfItems_ThenExceptionThrown()
    {
        var repository = Substitute.For<ITodoItemRepository>();
        repository.GetByExpressionAsync(i => i.TelegramUserId == 2).ReturnsForAnyArgs(TodoItemsBuilder.Generate(2, 7));

        var sut = CreateSut(repository);

        var createModel = new CreateTodoItemDto
        {
            Text = "text",
            UserId = 2,
        };

        var action = async () =>
        {
            await sut.CreateAsync(createModel);
        };

        await action.Should().ThrowAsync<TodoItemLimitException>();
    }

    [Fact]
    public async Task CreateAsync_WhenTestIsEmpty_ThenValidationFails()
    {
        var repository = Substitute.For<ITodoItemRepository>();
        var sut = CreateSut(repository);

        var createModel = new CreateTodoItemDto
        {
            Text = string.Empty,
            UserId = 2,
        };

        var action = async () =>
        {
            await sut.CreateAsync(createModel);
        };

        await action.Should().ThrowAsync<TodoItemValidationException>();
    }

    [Fact]
    public async Task GetUserItemsAsync_WhenUserHasFiveItems_ThenListOfFiveItems_Returned()
    {
        var repository = Substitute.For<ITodoItemRepository>();
        repository.GetByExpressionAsync(i => i.TelegramUserId == 3).ReturnsForAnyArgs(TodoItemsBuilder.Generate(3, 5));

        var sut = CreateSut(repository);

        var result = await sut.GetUserItemsAsync(3);

        result.Count.Should().Be(5);
        result.All(i => i.TelegramUserId == 3).Should().BeTrue();
    }

    private static ITodoItemService CreateSut(ITodoItemRepository repository)
    {
        var validator = new CreateTodoItemValidator();
        var mapper = new TodoItemMapper();

        return new TodoItemService(repository, validator, mapper);
    }
}
