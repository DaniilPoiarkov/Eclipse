namespace Eclipse.Application.Contracts.TodoItems;

public interface ITodoItemService
{
    IReadOnlyList<TodoItemDto> GetUserItems(long userId);

    TodoItemDto AddItem(long userId, string text);

    void FinishItem(Guid itemId);
}
