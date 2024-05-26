using Eclipse.Application.Contracts.Users;
using Eclipse.Common.Results;

namespace Eclipse.Application.Contracts.TodoItems;

public interface ITodoItemService
{
    Task<Result<UserDto>> CreateAsync(CreateTodoItemDto input, CancellationToken cancellationToken = default);

    Task<Result<UserDto>> FinishItemAsync(long chatId, Guid itemId, CancellationToken cancellationToken = default);
}
