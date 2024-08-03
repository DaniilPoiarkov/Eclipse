using Bogus;

using Eclipse.Application.Contracts.TodoItems;
using Eclipse.Application.TodoItems;
using Eclipse.Domain.Shared.Errors;
using Eclipse.Domain.Shared.TodoItems;
using Eclipse.Domain.TodoItems;
using Eclipse.Domain.Users;
using Eclipse.Tests.Generators;
using Eclipse.Tests.Utils;

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
        var user = UserGenerator.Get();

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
        var user = CreateUser(7);

        _repository.GetByExpressionAsync(_ => true)
            .ReturnsForAnyArgs(Task.FromResult<IReadOnlyList<User>>([user]));

        var createModel = new CreateTodoItemDto
        {
            Text = "text",
        };

        var result = await Sut.CreateAsync(user.ChatId, createModel);

        result.IsSuccess.Should().BeFalse();

        var error = result.Error;
        ErrorComparer.AreEqual(error, expectedError);
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

        ErrorComparer.AreEqual(expectedError, error);
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

        ErrorComparer.AreEqual(expectedError, error);
        error.Args.Should().BeEquivalentTo(expectedError.Args);
    }

    [Theory]
    [InlineData(3)]
    [InlineData(1)]
    [InlineData(6)]
    public async Task GetListAsync_WhenUserHasTodoItems_ThenAllItemsReturned(int todoItemsCount)
    {
        var user = CreateUser(todoItemsCount);

        _repository.FindAsync(user.Id)
            .ReturnsForAnyArgs(Task.FromResult<User?>(user));

        var result = await Sut.GetListAsync(user.Id);

        result.IsSuccess.Should().BeTrue();
        result.Value.Count.Should().Be(todoItemsCount);
        result.Value.All(item => item.UserId == user.Id).Should().BeTrue();
        result.Value.Select(item => IsValidTodoItem(item, user)).All(x => x).Should().BeTrue();
    }

    [Fact]
    public async Task GetAsync_WhenTodoItemExists_ThenItReturned()
    {
        var user = CreateUser(1);

        _repository.FindAsync(user.Id)
            .ReturnsForAnyArgs(Task.FromResult<User?>(user));

        var result = await Sut.GetAsync(user.Id, user.TodoItems.First().Id);

        result.IsSuccess.Should().BeTrue();
        IsValidTodoItem(result.Value, user).Should().BeTrue();
    }

    [Fact]
    public async Task GetAsync_WhenTodoItemNotFound_ThenErrorReturned()
    {
        var expectedError = DefaultErrors.EntityNotFound(typeof(TodoItem));
        var user = CreateUser(0);

        _repository.GetByExpressionAsync(_ => true)
            .ReturnsForAnyArgs(Task.FromResult<IReadOnlyList<User>>([user]));

        var result = await Sut.GetAsync(user.Id, Guid.NewGuid());

        result.IsSuccess.Should().BeFalse();
        ErrorComparer.AreEqual(result.Error, expectedError);
    }

    private static bool IsValidTodoItem(TodoItemDto item, User user)
    {
        return user.GetTodoItem(item.Id) is TodoItem todoItem
            && todoItem.Text == item.Text
            && todoItem.UserId == user.Id
            && todoItem.Id != Guid.Empty;
    }

    private static User CreateUser(int todoItemsCount)
    {
        var user = UserGenerator.Get();

        var faker = new Faker();

        for (int i = 0; i < todoItemsCount; i++)
        {
            user.AddTodoItem(faker.Lorem.Word());
        }

        return user;
    }
}
