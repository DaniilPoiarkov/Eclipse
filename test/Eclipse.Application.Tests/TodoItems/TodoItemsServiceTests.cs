﻿using Bogus;

using Eclipse.Application.Contracts.TodoItems;
using Eclipse.Application.TodoItems;
using Eclipse.Domain.Shared.Errors;
using Eclipse.Domain.Shared.TodoItems;
using Eclipse.Domain.TodoItems;
using Eclipse.Domain.Users;
using Eclipse.Tests.Generators;

using FluentAssertions;

using NSubstitute;

using Xunit;

namespace Eclipse.Application.Tests.TodoItems;

public sealed class TodoItemsServiceTests
{
    private readonly IUserRepository _repository;

    private readonly Lazy<ITodoItemService> _lazySut;

    private ITodoItemService Sut => _lazySut.Value;

    public TodoItemsServiceTests()
    {
        _repository = Substitute.For<IUserRepository>();
        _lazySut = new Lazy<ITodoItemService>(
            () => new TodoItemService(
                new UserManager(_repository)
            ));
    }

    [Fact]
    public async Task CreateAsync_WhenInputValid_ThenCreatedSuccessfully()
    {
        var user = UserGenerator.Generate(1).First();

        _repository.GetByExpressionAsync(_ => true)
            .ReturnsForAnyArgs(Task.FromResult<IReadOnlyList<User>>([user]));

        var createModel = new CreateTodoItemDto
        {
            Text = "text",
        };

        var result = await Sut.CreateAsync(user.ChatId, createModel);

        result.IsSuccess.Should().BeTrue();

        var dto = result.Value;
        dto.TodoItems.Count.Should().Be(1);

        var todoItem = dto.TodoItems[0];

        todoItem.Text.Should().Be(createModel.Text);
        todoItem.UserId.Should().Be(user.Id);
        todoItem.Id.Should().NotBeEmpty();
    }

    [Fact]
    public async Task CreateAsync_WhenUserReachLimitOfItems_ThenFailureResultReturned()
    {
        var expectedError = UserDomainErrors.TodoItemsLimit(TodoItemConstants.Limit);
        var user = UserGenerator.Generate(1).First();

        var faker = new Faker();

        for (int i = 0; i < 7; i++)
        {
            user.AddTodoItem(faker.Lorem.Word());
        }

        _repository.GetByExpressionAsync(_ => true)
            .ReturnsForAnyArgs(Task.FromResult<IReadOnlyList<User>>([user]));

        var createModel = new CreateTodoItemDto
        {
            Text = "text",
        };

        var result = await Sut.CreateAsync(user.ChatId, createModel);

        result.IsSuccess.Should().BeFalse();

        var error = result.Error;
        error.Code.Should().Be(expectedError.Code);
        error.Description.Should().Be(expectedError.Description);
        error.Args.Should().BeEquivalentTo(expectedError.Args);
    }

    [Fact]
    public async Task CreateAsync_WhenTextIsEmpty_ThenFailureResultReturned()
    {
        var expectedError = TodoItemDomainErrors.TodoItemIsEmpty();
        var user = UserGenerator.Generate(1).First();

        _repository.GetByExpressionAsync(_ => true)
            .ReturnsForAnyArgs(Task.FromResult<IReadOnlyList<User>>([user]));

        var createModel = new CreateTodoItemDto
        {
            Text = string.Empty,
        };

        var result = await Sut.CreateAsync(user.ChatId, createModel);

        result.IsSuccess.Should().BeFalse();
        var error = result.Error;

        error.Code.Should().Be(expectedError.Code);
        error.Description.Should().Be(expectedError.Description);
        error.Args.Should().BeEquivalentTo(expectedError.Args);
    }

    [Fact]
    public async Task CreateAsync_WhenUserNotExists_ThenFailureResultReturned()
    {
        var expectedError = DefaultErrors.EntityNotFound(typeof(User));

        var createModel = new CreateTodoItemDto
        {
            Text = "text",
        };

        var result = await Sut.CreateAsync(2, createModel);

        result.IsSuccess.Should().BeFalse();
        var error = result.Error;

        error.Code.Should().Be(expectedError.Code);
        error.Description.Should().Be(expectedError.Description);
        error.Args.Should().BeEquivalentTo(expectedError.Args);
    }
}
