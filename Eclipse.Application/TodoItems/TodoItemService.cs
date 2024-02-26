using Eclipse.Application.Contracts.Base;
using Eclipse.Application.Contracts.IdentityUsers;
using Eclipse.Application.Contracts.TodoItems;
using Eclipse.Common.Results;
using Eclipse.Domain.IdentityUsers;
using Eclipse.Domain.Shared.Errors;

namespace Eclipse.Application.TodoItems;

internal sealed class TodoItemService : ITodoItemService
{
    private readonly IdentityUserManager _userManager;

    private readonly IMapper<IdentityUser, IdentityUserDto> _mapper;

    public TodoItemService(
        IdentityUserManager userManager,
        IMapper<IdentityUser, IdentityUserDto> mapper)
    {
        _userManager = userManager;
        _mapper = mapper;
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

        return _mapper.Map(user);
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

        return _mapper.Map(user);
    }
}
