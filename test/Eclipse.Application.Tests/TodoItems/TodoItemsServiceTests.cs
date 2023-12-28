using Bogus;

using Eclipse.Application.Contracts.TodoItems;
using Eclipse.Application.Exceptions;
using Eclipse.Application.IdentityUsers;
using Eclipse.Application.TodoItems;
using Eclipse.Domain.IdentityUsers;
using Eclipse.Domain.TodoItems;
using Eclipse.Tests.Generators;

using FluentAssertions;

using NSubstitute;

using Xunit;

namespace Eclipse.Application.Tests.TodoItems;

public class TodoItemsServiceTests
{
    private readonly IdentityUserManager _userManager;

    private readonly Lazy<ITodoItemService> _lazySut;

    private ITodoItemService Sut => _lazySut.Value;

    public TodoItemsServiceTests()
    {
        var mapper = new IdentityUserMapper();

        _userManager = Substitute.For<IdentityUserManager>(Substitute.For<IIdentityUserRepository>());
        _lazySut = new Lazy<ITodoItemService>(() => new TodoItemService(_userManager, mapper));
    }

    [Fact]
    public async Task CreateAsync_WhenInputValid_ThenCreatedSuccessfully()
    {   
        var user = IdentityUserGenerator.Generate(1).First();

        _userManager.FindByChatIdAsync(user.ChatId).ReturnsForAnyArgs(Task.FromResult<IdentityUser?>(user));

        var createModel = new CreateTodoItemDto
        {
            Text = "text",
            UserId = user.ChatId,
        };

        var result = await Sut.CreateAsync(createModel);

        result.Should().NotBeNull();

        result.TodoItems.Count.Should().Be(1);

        var todoItem = result.TodoItems[0];

        todoItem.Text.Should().Be(createModel.Text);
        todoItem.UserId.Should().Be(user.Id);
        todoItem.Id.Should().NotBeEmpty();
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
