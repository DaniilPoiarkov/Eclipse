using Bogus;

using Eclipse.Application.Contracts.TodoItems;
using Eclipse.Application.Localizations;
using Eclipse.Application.TodoItems;
using Eclipse.Domain.Shared.Errors;
using Eclipse.Domain.Shared.TodoItems;
using Eclipse.Domain.TodoItems;
using Eclipse.Domain.Users;
using Eclipse.Tests.Builders;
using Eclipse.Tests.Generators;
using Eclipse.Tests.Utils;

using FluentAssertions;

using Microsoft.Extensions.Localization;

using NSubstitute;

using Xunit;

namespace Eclipse.Application.Tests.TodoItems;

public sealed class TodoItemsServiceTests
{
    private readonly IUserRepository _repository;

    private readonly IStringLocalizer<TodoItemService> _localizer;

    private readonly TodoItemService _sut;

    public TodoItemsServiceTests()
    {
        _repository = Substitute.For<IUserRepository>();
        _localizer = Substitute.For<IStringLocalizer<TodoItemService>>();
        _sut = new TodoItemService(new UserManager(_repository), _localizer);
    }

    [Theory]
    [InlineData("test")]
    [InlineData("12340nnkjcasclk")]
    [InlineData("Something in the way")]
    [InlineData("testtesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttest")]
    [InlineData("Some regular text! With __dif3r3nt &^% characters!)(_++_*@")]
    public async Task CreateAsync_WhenInputValid_ThenCreatedSuccessfully(string text)
    {
        var user = UserGenerator.Get();

        _repository.GetByExpressionAsync(_ => true)
            .ReturnsForAnyArgs(Task.FromResult<IReadOnlyList<User>>([user]));

        var createModel = new CreateTodoItemDto
        {
            Text = text,
        };

        var result = await _sut.CreateAsync(user.ChatId, createModel);

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
        LocalizerBuilder<TodoItemService>.Configure(_localizer)
            .ForWithArgs("TodoItem:Limit", TodoItemConstants.Limit)
            .Return($"The limit of {TodoItemConstants.Limit} reached.");

        var expectedError = UserDomainErrors.TodoItemsLimit(TodoItemConstants.Limit)
            .ToLocalized(_localizer);

        var user = CreateUser(TodoItemConstants.Limit);

        _repository.GetByExpressionAsync(_ => true)
            .ReturnsForAnyArgs([user]);

        var createModel = new CreateTodoItemDto
        {
            Text = "text",
        };

        var result = await _sut.CreateAsync(user.ChatId, createModel);

        result.IsSuccess.Should().BeFalse();

        ErrorComparer.AreEqual(result.Error, expectedError);
    }

    [Theory]
    [InlineData("")]
    [InlineData("        ")]
    public async Task CreateAsync_WhenTextIsInvalid_ThenErrorReturned(string text)
    {
        LocalizerBuilder<TodoItemService>.Configure(_localizer)
            .For("TodoItem:MaxLength")
            .Return("Todo item is empty");

        var expectedError = TodoItemDomainErrors.TodoItemIsEmpty()
            .ToLocalized(_localizer);

        var user = UserGenerator.Get();

        _repository.GetByExpressionAsync(_ => true)
            .ReturnsForAnyArgs(Task.FromResult<IReadOnlyList<User>>([user]));

        var createModel = new CreateTodoItemDto
        {
            Text = text,
        };

        var result = await _sut.CreateAsync(user.ChatId, createModel);

        result.IsSuccess.Should().BeFalse();
        ErrorComparer.AreEqual(expectedError, result.Error);
    }

    [Fact]
    public async Task CreateAsync_WhenUserNotExists_ThenFailureResultReturned()
    {
        LocalizerBuilder<TodoItemService>.Configure(_localizer)
            .ForWithArgs("Entity:NotFound", typeof(User).Name)
            .Return("User not found");

        var expectedError = DefaultErrors.EntityNotFound(typeof(User), _localizer);

        var createModel = new CreateTodoItemDto
        {
            Text = "text",
        };

        var result = await _sut.CreateAsync(2, createModel);

        result.IsSuccess.Should().BeFalse();
        ErrorComparer.AreEqual(expectedError, result.Error);
    }

    [Theory]
    [InlineData(3)]
    [InlineData(1)]
    [InlineData(6)]
    public async Task GetListAsync_WhenUserHasTodoItems_ThenAllItemsReturned(int todoItemsCount)
    {
        var user = CreateUser(todoItemsCount);

        _repository.FindAsync(user.Id)
            .Returns(user);

        var result = await _sut.GetListAsync(user.Id);

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
            .Returns(user);

        var result = await _sut.GetAsync(user.Id, user.TodoItems.First().Id);

        result.IsSuccess.Should().BeTrue();
        IsValidTodoItem(result.Value, user).Should().BeTrue();
    }

    [Fact]
    public async Task GetAsync_WhenTodoItemNotFound_ThenErrorReturned()
    {
        LocalizerBuilder<TodoItemService>.Configure(_localizer)
            .ForWithArgs("Entity:NotFound", typeof(TodoItem).Name)
            .Return("Todo item not found");

        var expectedError = DefaultErrors.EntityNotFound(typeof(TodoItem), _localizer);
        var user = CreateUser(0);

        _repository.FindAsync(user.Id).Returns(user);

        var result = await _sut.GetAsync(user.Id, Guid.NewGuid());

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
