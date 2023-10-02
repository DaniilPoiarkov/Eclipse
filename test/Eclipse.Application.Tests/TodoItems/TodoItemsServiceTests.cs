﻿using Eclipse.Application.Contracts.TodoItems;
using Eclipse.Application.TodoItems;
using Eclipse.Application.TodoItems.Exceptions;
using Eclipse.Domain.TodoItems;
using Eclipse.Tests.Generators;
using FluentAssertions;

using NSubstitute;

using Xunit;

namespace Eclipse.Application.Tests.TodoItems;

public class TodoItemsServiceTests
{
    private readonly ITodoItemRepository _repository;

    private readonly Lazy<ITodoItemService> _lazySut;

    private ITodoItemService Sut => _lazySut.Value;

    public TodoItemsServiceTests()
    {
        var validator = new CreateTodoItemValidator();
        var mapper = new TodoItemMapper();

        _repository = Substitute.For<ITodoItemRepository>();
        _lazySut = new Lazy<ITodoItemService>(() => new TodoItemService(_repository, validator, mapper));
    }

    [Fact]
    public async Task CreateAsync_WhenInputValid_ThenCreatedSuccessfully()
    {
        _repository.GetByExpressionAsync(i => i.TelegramUserId == 1).ReturnsForAnyArgs(new List<TodoItem>());

        var createModel = new CreateTodoItemDto
        {
            Text = "text",
            UserId = 1,
        };

        var result = await Sut.CreateAsync(createModel);

        result.Should().NotBeNull();
        result.Text.Should().Be(createModel.Text);
        result.TelegramUserId.Should().Be(createModel.UserId);
        result.Id.Should().NotBeEmpty();
    }

    [Fact]
    public async Task CreateAsync_WhenUserReachLimitOfItems_ThenExceptionThrown()
    {
        _repository.GetByExpressionAsync(i => i.TelegramUserId == 2).ReturnsForAnyArgs(TodoItemsGenerator.Generate(2, 7));

        var createModel = new CreateTodoItemDto
        {
            Text = "text",
            UserId = 2,
        };

        var action = async () =>
        {
            await Sut.CreateAsync(createModel);
        };

        await action.Should().ThrowAsync<TodoItemLimitException>();
    }

    [Fact]
    public async Task CreateAsync_WhenTestIsEmpty_ThenValidationFails()
    {
        var createModel = new CreateTodoItemDto
        {
            Text = string.Empty,
            UserId = 2,
        };

        var action = async () =>
        {
            await Sut.CreateAsync(createModel);
        };

        await action.Should().ThrowAsync<TodoItemValidationException>();
    }

    [Fact]
    public async Task GetUserItemsAsync_WhenUserHasFiveItems_ThenListOfFiveItems_Returned()
    {
        _repository.GetByExpressionAsync(i => i.TelegramUserId == 3).ReturnsForAnyArgs(TodoItemsGenerator.Generate(3, 5));

        var result = await Sut.GetUserItemsAsync(3);

        result.Count.Should().Be(5);
        result.All(i => i.TelegramUserId == 3).Should().BeTrue();
    }
}
