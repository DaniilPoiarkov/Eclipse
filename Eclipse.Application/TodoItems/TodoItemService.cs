using Eclipse.Application.Contracts.IdentityUsers;
using Eclipse.Application.Contracts.TodoItems;
using Eclipse.Application.IdentityUsers;
using Eclipse.Common.Results;
using Eclipse.Domain.IdentityUsers;
using Eclipse.Domain.Shared.Errors;

namespace Eclipse.Application.TodoItems;

internal sealed class TodoItemService : ITodoItemService
{
    private readonly IdentityUserManager _userManager;

    public TodoItemService(IdentityUserManager userManager)
    {
        _userManager = userManager;
    }

    public async Task<Result<IdentityUserDto>> CreateAsync(CreateTodoItemDto input, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByChatIdAsync(input.UserId, cancellationToken);

        if (user is null)
        {
            return DefaultErrors.EntityNotFound(typeof(IdentityUser));
        }

        var result = user.AddTodoItem(input.Text);

        if (!result.IsSuccess)
        {
            return result.Error;
        }

        await _userManager.UpdateAsync(user, cancellationToken);

        return user.ToDto();
    }

    public async Task<Result<IdentityUserDto>> FinishItemAsync(long chatId, Guid itemId, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByChatIdAsync(chatId, cancellationToken);

        if (user is null)
        {
            return DefaultErrors.EntityNotFound(typeof(IdentityUser));
        }

        user.FinishItem(itemId);

        await _userManager.UpdateAsync(user, cancellationToken);

        return user.ToDto();
    }
}
