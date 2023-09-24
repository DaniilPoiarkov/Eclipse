namespace Eclipse.Application.TodoItems.Exceptions;

public class TodoItemLimitException : TodoItemException
{
    public TodoItemLimitException(int limit)
        : base("TodoItem:Limit", $"{limit}") { }
}
