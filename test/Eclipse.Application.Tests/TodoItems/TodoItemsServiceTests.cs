using Bogus;

using Eclipse.Application.Contracts.TodoItems;
using Eclipse.Application.IdentityUsers;
using Eclipse.Application.TodoItems;
using Eclipse.Domain.Exceptions;
using Eclipse.Domain.IdentityUsers;
using Eclipse.Domain.TodoItems;
using Eclipse.Tests.Generators;

using FluentAssertions;

using NSubstitute;

using Xunit;

namespace Eclipse.Application.Tests.TodoItems;

public sealed class TodoItemsServiceTests
{
    private readonly IIdentityUserRepository _repository;

    private readonly Lazy<ITodoItemService> _lazySut;

    private ITodoItemService Sut => _lazySut.Value;

    public TodoItemsServiceTests()
    {
        var mapper = new IdentityUserMapper();

        _repository = Substitute.For<IIdentityUserRepository>();
        _lazySut = new Lazy<ITodoItemService>(() => new TodoItemService(new IdentityUserManager(_repository), mapper));
    }

    [Fact]
    public async Task CreateAsync_WhenInputValid_ThenCreatedSuccessfully()
    {   
        var user = IdentityUserGenerator.Generate(1).First();

        _repository.GetByExpressionAsync(_ => true)
            .ReturnsForAnyArgs(Task.FromResult<IReadOnlyList<IdentityUser>>([user]));

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

        _repository.GetByExpressionAsync(_ => true)
            .ReturnsForAnyArgs(Task.FromResult<IReadOnlyList<IdentityUser>>([user]));

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

        _repository.GetByExpressionAsync(_ => true)
            .ReturnsForAnyArgs(Task.FromResult<IReadOnlyList<IdentityUser>>([user]));

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
    public async Task CreateAsync_WhenUserNotExists_ThenEntityNotFoundExceptionThrown()
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

        await action.Should().ThrowAsync<EntityNotFoundException>();
    }
}
