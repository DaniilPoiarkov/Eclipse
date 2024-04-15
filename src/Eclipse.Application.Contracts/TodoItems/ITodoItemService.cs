using Eclipse.Application.Contracts.IdentityUsers;
using Eclipse.Common.Results;

namespace Eclipse.Application.Contracts.TodoItems;

public interface ITodoItemService
{
    Task<Result<IdentityUserDto>> CreateAsync(CreateTodoItemDto input, CancellationToken cancellationToken = default);

    Task<Result<IdentityUserDto>> FinishItemAsync(long chatId, Guid itemId, CancellationToken cancellationToken = default);
}
