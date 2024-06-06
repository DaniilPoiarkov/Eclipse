using Eclipse.Application.Contracts.TodoItems;
using Eclipse.Application.Contracts.Users;
using Eclipse.Application.Users;
using Eclipse.Common.Results;
using Eclipse.Domain.Shared.Errors;
using Eclipse.Domain.Users;

namespace Eclipse.Application.TodoItems;

internal sealed class TodoItemService : ITodoItemService
{
    private readonly UserManager _userManager;

    public TodoItemService(UserManager userManager)
    {
        _userManager = userManager;
    }

    public async Task<Result<UserDto>> CreateAsync(CreateTodoItemDto input, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByChatIdAsync(input.UserId, cancellationToken);

        if (user is null)
        {
            return DefaultErrors.EntityNotFound(typeof(User));
        }

        var result = user.AddTodoItem(input.Text);

        if (!result.IsSuccess)
        {
            return result.Error;
        }

        await _userManager.UpdateAsync(user, cancellationToken);

        return user.ToDto();
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
}
