using Bogus;

using Eclipse.Application.Contracts.TodoItems;
using Eclipse.Application.Exceptions;
using Eclipse.Application.TodoItems;
using Eclipse.Domain.IdentityUsers;
using Eclipse.Domain.Shared.TodoItems;
using Eclipse.Domain.TodoItems;
using Eclipse.Tests.Generators;

using FluentAssertions;

using NSubstitute;

using Xunit;

namespace Eclipse.Application.Tests.TodoItems;

public class TodoItemsServiceTests
{
    private readonly ITodoItemRepository _repository;

    private readonly IdentityUserManager _userManager;

    private readonly Lazy<ITodoItemService> _lazySut;

    private ITodoItemService Sut => _lazySut.Value;

    public TodoItemsServiceTests()
    {
        var mapper = new TodoItemMapper();

        _repository = Substitute.For<ITodoItemRepository>();
        _userManager = Substitute.For<IdentityUserManager>(Substitute.For<IIdentityUserRepository>());
        _lazySut = new Lazy<ITodoItemService>(() => new TodoItemService(_repository, _userManager, mapper));
    }

    [Fact]
    public async Task CreateAsync_WhenInputValid_ThenCreatedSuccessfully()
    {
        _repository.GetByExpressionAsync(i => i.TelegramUserId == 1).ReturnsForAnyArgs(new List<TodoItem>());
        
        var user = IdentityUserGenerator.Generate(1).First();

        _userManager.FindByChatIdAsync(user.ChatId).ReturnsForAnyArgs(Task.FromResult<IdentityUser?>(user));

        var createModel = new CreateTodoItemDto
        {
            Text = "text",
            UserId = user.ChatId,
        };

        var result = await Sut.CreateAsync(createModel);

        result.Should().NotBeNull();
        result.Text.Should().Be(createModel.Text);
        result.TelegramUserId.Should().Be(user.ChatId);
        result.Id.Should().NotBeEmpty();
    }

    [Fact]
    public async Task CreateAsync_WhenUserReachLimitOfItems_ThenExceptionThrown()
    {
        var user = IdentityUserGenerator.Generate(1).First();

        var faker = new Faker();

        for (int i = 0; i < 7; i++)
        {
            user.AddTodoItem(faker.Lorem.Word());
        }

        _repository.GetByExpressionAsync(i => i.TelegramUserId == user.ChatId).ReturnsForAnyArgs(TodoItemsGenerator.Generate(user.ChatId, 7));
        _userManager.FindByChatIdAsync(user.ChatId).ReturnsForAnyArgs(Task.FromResult<IdentityUser?>(user));

        var createModel = new CreateTodoItemDto
        {
            Text = "text",
            UserId = user.ChatId,
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
        var user = IdentityUserGenerator.Generate(1).First();

        _userManager.FindByChatIdAsync(user.ChatId).Returns(Task.FromResult<IdentityUser?>(user));

        var createModel = new CreateTodoItemDto
        {
            Text = string.Empty,
            UserId = user.ChatId,
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

    [Fact]
    public async Task CreateAsync_WhenUserNotExists_ThenObjectNotFoundExceptionThrown()
    {
        var createModel = new CreateTodoItemDto
        {
            Text = "text",
            UserId = 2,
        };

        var action = async () =>
        {
            await Sut.CreateAsync(createModel);
        };

        await action.Should().ThrowAsync<ObjectNotFoundException>();
    }
}
