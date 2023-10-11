using Eclipse.Application.Contracts.IdentityUsers;

namespace Eclipse.Application.Contracts.TodoItems;

public interface ITodoItemService
{
    Task<IdentityUserDto> CreateAsync(CreateTodoItemDto input, CancellationToken cancellationToken = default);

    Task<IdentityUserDto> FinishItemAsync(long chatId, Guid itemId, CancellationToken cancellationToken = default);
}
