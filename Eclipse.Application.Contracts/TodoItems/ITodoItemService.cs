namespace Eclipse.Application.Contracts.TodoItems;

public interface ITodoItemService
{
    IReadOnlyList<TodoItemDto> GetUserItems(long userId);

    TodoItemDto AddItem(CreateTodoItemDto input);

    void FinishItem(Guid itemId);
}
