using Bogus;

using Eclipse.Application.Contracts.TodoItems;
using Eclipse.Application.TodoItems;
using Eclipse.Common.Clock;
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

    private readonly ITimeProvider _timeProvider;

    private readonly TodoItemService _sut;

    public TodoItemsServiceTests()
    {
        _repository = Substitute.For<IUserRepository>();
        _timeProvider = Substitute.For<ITimeProvider>();

        _sut = new TodoItemService(_repository, _timeProvider);
    }

    [Fact]
    public async Task GetAsync_WhenUserNotExist_ThenErrorReturned()
    {
        var expected = DefaultErrors.EntityNotFound<User>();

        var result = await _sut.GetAsync(Guid.NewGuid(), Guid.NewGuid());

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task GetListAsync_WhenUserNotExist_ThenErrorReturned()
    {
        var expected = DefaultErrors.EntityNotFound<User>();

        var result = await _sut.GetListAsync(Guid.NewGuid());

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task FinishItemAsync_WhenItemCanBeFinished_ThenSuccessfullyFinished()
    {
        var user = UserGenerator.Get();

        var todoItem = user.AddTodoItem("test", DateTime.UtcNow);

        _repository.FindByChatIdAsync(user.ChatId).Returns(user);

        var result = await _sut.FinishAsync(user.ChatId, todoItem.Value.Id);

        result.IsSuccess.Should().BeTrue();
        result.Value.TodoItems.Should().BeEmpty();
    }

    [Fact]
    public async Task FinishItemAsync_WhenUserNotExist_ThenErrorReturned()
    {
        var expected = DefaultErrors.EntityNotFound<User>();

        var result = await _sut.FinishAsync(1, Guid.NewGuid());

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task FinishItemAsync_WhenItemNotExists_ThenErrorReturned()
    {
        var user = UserGenerator.Get();

        _repository.FindByChatIdAsync(user.ChatId).Returns(user);

        var expected = UserDomainErrors.TodoItemNotFound();

        var result = await _sut.FinishAsync(user.ChatId, Guid.NewGuid());

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().BeEquivalentTo(expected);
    }

    [Theory]
    [InlineData("test")]
    [InlineData("12340nnkjcasclk")]
    [InlineData("Something in the way")]
    [InlineData("testtesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttest")]
    [InlineData("Some regular text! With __dif3r3nt &^% characters!)(_++_*@")]
    public async Task CreateAsync_WhenInputValidAndIdSpecified_ThenCreatedSuccessfully(string text)
    {
        var user = UserGenerator.Get();

        _repository.FindAsync(user.Id).Returns(user);

        var createModel = new CreateTodoItemDto(text);

        var result = await _sut.CreateAsync(user.Id, createModel);

        result.IsSuccess.Should().BeTrue();

        var todoItem = result.Value;

        todoItem.Text.Should().Be(createModel.Text);
        todoItem.UserId.Should().Be(user.Id);
        todoItem.Id.Should().NotBeEmpty();
    }

    [Fact]
    public async Task CreateAsync_WhenUserReachLimitOfItemsAndIdSpecified_ThenFailureResultReturned()
    {
        var expected = UserDomainErrors.TodoItemsLimit(TodoItemConstants.Limit);
        var user = CreateUser(7);

        _repository.FindAsync(user.Id).Returns(user);

        var createModel = new CreateTodoItemDto("text");

        var result = await _sut.CreateAsync(user.Id, createModel);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().BeEquivalentTo(expected);
    }

    [Theory]
    [InlineData("")]
    [InlineData("        ")]
    public async Task CreateAsync_WhenTextIsInvalidAndIdSpecified_ThenErrorReturned(string text)
    {
        var expected = TodoItemDomainErrors.TodoItemIsEmpty();
        var user = UserGenerator.Get();

        _repository.FindAsync(user.Id).Returns(user);

        var createModel = new CreateTodoItemDto(text);

        var result = await _sut.CreateAsync(user.Id, createModel);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task CreateAsync_WhenUserNotExistsAndIdSpecified_ThenFailureResultReturned()
    {
        var expected = DefaultErrors.EntityNotFound<User>();

        var createModel = new CreateTodoItemDto("text");

        var result = await _sut.CreateAsync(Guid.NewGuid(), createModel);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().BeEquivalentTo(expected);
    }

    [Theory]
    [InlineData("test")]
    [InlineData("12340nnkjcasclk")]
    [InlineData("Something in the way")]
    [InlineData("testtesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttest")]
    [InlineData("Some regular text! With __dif3r3nt &^% characters!)(_++_*@")]
    public async Task CreateAsync_WhenInputValidAndChatIdSpecified_ThenCreatedSuccessfully(string text)
    {
        var user = UserGenerator.Get();

        _repository.FindByChatIdAsync(user.ChatId).Returns(user);

        var createModel = new CreateTodoItemDto(text);

        var result = await _sut.CreateAsync(user.ChatId, createModel);

        result.IsSuccess.Should().BeTrue();

        var todoItem = result.Value;

        todoItem.Text.Should().Be(createModel.Text);
        todoItem.UserId.Should().Be(user.Id);
        todoItem.Id.Should().NotBeEmpty();
    }

    [Fact]
    public async Task CreateAsync_WhenUserReachLimitOfItemsAndChatIdSpecified_ThenFailureResultReturned()
    {
        var expected = UserDomainErrors.TodoItemsLimit(TodoItemConstants.Limit);
        var user = CreateUser(7);

        _repository.FindByChatIdAsync(user.ChatId).Returns(user);

        var createModel = new CreateTodoItemDto("text");

        var result = await _sut.CreateAsync(user.ChatId, createModel);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().BeEquivalentTo(expected);
    }

    [Theory]
    [InlineData("")]
    [InlineData("        ")]
    public async Task CreateAsync_WhenTextIsInvalidAndChatIdSpecified_ThenErrorReturned(string text)
    {
        var expected = TodoItemDomainErrors.TodoItemIsEmpty();
        var user = UserGenerator.Get();

        _repository.FindByChatIdAsync(user.ChatId).Returns(user);

        var createModel = new CreateTodoItemDto(text);

        var result = await _sut.CreateAsync(user.ChatId, createModel);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().BeEquivalentTo(expected);
    }

    [Theory]
    [InlineData(2, "text")]
    public async Task CreateAsync_WhenUserNotExistsAndChatIdSpecified_ThenFailureResultReturned(long chatId, string text)
    {
        var expected = DefaultErrors.EntityNotFound<User>();

        var createModel = new CreateTodoItemDto(text);

        var result = await _sut.CreateAsync(chatId, createModel);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().BeEquivalentTo(expected);
    }

    [Theory]
    [InlineData(3)]
    [InlineData(1)]
    [InlineData(6)]
    public async Task GetListAsync_WhenUserHasTodoItems_ThenAllItemsReturned(int todoItemsCount)
    {
        var user = CreateUser(todoItemsCount);

        _repository.FindAsync(user.Id).Returns(user);

        var result = await _sut.GetListAsync(user.Id);

        result.Value.Count.Should().Be(todoItemsCount);
        result.Value.Select(item => IsValidTodoItem(item, user)).All(x => x).Should().BeTrue();
    }

    [Fact]
    public async Task GetAsync_WhenTodoItemExists_ThenItReturned()
    {
        var user = CreateUser(1);

        _repository.FindAsync(user.Id)
            .ReturnsForAnyArgs(Task.FromResult<User?>(user));

        var result = await _sut.GetAsync(user.Id, user.TodoItems.First().Id);

        result.IsSuccess.Should().BeTrue();
        IsValidTodoItem(result.Value, user).Should().BeTrue();
    }

    [Fact]
    public async Task GetAsync_WhenTodoItemNotFound_ThenErrorReturned()
    {
        var expected = DefaultErrors.EntityNotFound<TodoItem>();
        var user = CreateUser(0);

        _repository.FindAsync(user.Id).Returns(user);

        var result = await _sut.GetAsync(user.Id, Guid.NewGuid());

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().BeEquivalentTo(expected);
    }

    private static bool IsValidTodoItem(TodoItemDto item, User user)
    {
        return user.TodoItems.FirstOrDefault(i => i.Id == item.Id) is TodoItem todoItem
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
            user.AddTodoItem(faker.Lorem.Word(), faker.Date.Past());
        }

        return user;
    }
}
