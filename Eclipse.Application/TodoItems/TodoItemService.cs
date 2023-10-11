using Eclipse.Application.Contracts.Base;
using Eclipse.Application.Contracts.IdentityUsers;
using Eclipse.Application.Contracts.TodoItems;
using Eclipse.Application.Exceptions;
using Eclipse.Domain.IdentityUsers;
using Eclipse.Domain.TodoItems;

namespace Eclipse.Application.TodoItems;

internal class TodoItemService : ITodoItemService
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

    public async Task<IdentityUserDto> CreateAsync(CreateTodoItemDto input, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByChatIdAsync(input.UserId, cancellationToken)
            ?? throw new ObjectNotFoundException(nameof(IdentityUser));

        user.AddTodoItem(input.Text);

        await _userManager.UpdateAsync(user, cancellationToken);

        return _mapper.Map(user);
    }

    public async Task<IdentityUserDto> FinishItemAsync(long chatId, Guid itemId, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByChatIdAsync(chatId, cancellationToken)
            ?? throw new ObjectNotFoundException(nameof(IdentityUser));

        user.FinishItem(itemId);

        await _userManager.UpdateAsync(user, cancellationToken);

        return _mapper.Map(user);
    }
}
