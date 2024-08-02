using Eclipse.Application.Contracts.Users;
using Eclipse.Common.Results;

namespace Eclipse.Application.Contracts.TodoItems;

public interface ITodoItemService
{
    Task<Result<UserDto>> CreateAsync(long chatId, CreateTodoItemDto model, CancellationToken cancellationToken = default);

    Task<Result<TodoItemDto>> CreateAsync(Guid userId, CreateTodoItemDto model, CancellationToken cancellationToken = default);

    Task<Result<UserDto>> FinishItemAsync(long chatId, Guid itemId, CancellationToken cancellationToken = default);

    Task<Result<List<TodoItemDto>>> GetListAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<Result<TodoItemDto>> GetAsync(Guid userId, Guid todoItemId, CancellationToken cancellationToken = default);
}
