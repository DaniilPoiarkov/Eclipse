namespace Eclipse.Application.Contracts.TodoItems;

public interface ITodoItemService
{
    Task<IReadOnlyList<TodoItemDto>> GetUserItemsAsync(long userId, CancellationToken cancellationToken = default);

    Task<TodoItemDto> CreateAsync(CreateTodoItemDto input, CancellationToken cancellationToken = default);

    Task FinishItemAsync(Guid itemId, CancellationToken cancellationToken = default);
}
