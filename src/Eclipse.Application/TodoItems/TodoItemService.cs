using Eclipse.Application.Contracts.TodoItems;
using Eclipse.Application.Contracts.Users;
using Eclipse.Application.Users;
using Eclipse.Common.Results;
using Eclipse.Domain.Shared.Errors;
using Eclipse.Domain.TodoItems;
using Eclipse.Domain.Users;

namespace Eclipse.Application.TodoItems;

internal sealed class TodoItemService : ITodoItemService
{
    private readonly UserManager _userManager;

    public TodoItemService(UserManager userManager)
    {
        _userManager = userManager;
    }

    public async Task<Result<UserDto>> CreateAsync(long chatId, CreateTodoItemDto model, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByChatIdAsync(chatId, cancellationToken);

        if (user is null)
        {
            return DefaultErrors.EntityNotFound(typeof(User));
        }

        var result = await CreateAsync(user, model, cancellationToken);

        if (!result.IsSuccess)
        {
            return result.Error;
        }

        return user.ToDto();
    }

    public async Task<Result<TodoItemDto>> CreateAsync(Guid userId, CreateTodoItemDto model, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId, cancellationToken);

        if (user is null)
        {
            return DefaultErrors.EntityNotFound(typeof(User));
        }

        var result = await CreateAsync(user, model, cancellationToken);

        return result.Value.ToDto();
    }

    private async Task<Result<TodoItem>> CreateAsync(User user, CreateTodoItemDto model, CancellationToken cancellationToken)
    {
        var result = user.AddTodoItem(model.Text);

        if (result.IsSuccess)
        {
            await _userManager.UpdateAsync(user, cancellationToken);
        }

        return result;
    }

    public async Task<Result<UserDto>> FinishItemAsync(long chatId, Guid itemId, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByChatIdAsync(chatId, cancellationToken);

        if (user is null)
        {
            return DefaultErrors.EntityNotFound(typeof(User));
        }

        user.FinishItem(itemId);

        await _userManager.UpdateAsync(user, cancellationToken);

        return user.ToDto();
    }

    public async Task<Result<List<TodoItemDto>>> GetListAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId, cancellationToken);

        if (user is null)
        {
            return DefaultErrors.EntityNotFound(typeof(User));
        }

        return user.TodoItems.Select(item => item.ToDto()).ToList();
    }

    public async Task<Result<TodoItemDto>> GetAsync(Guid userId, Guid todoItemId, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId, cancellationToken);

        if (user is null)
        {
            return DefaultErrors.EntityNotFound(typeof(User));
        }

        var item = user.GetTodoItem(todoItemId);

        if (item is null)
        {
            return DefaultErrors.EntityNotFound(typeof(TodoItem));
        }

        return item.ToDto();
    }
}
