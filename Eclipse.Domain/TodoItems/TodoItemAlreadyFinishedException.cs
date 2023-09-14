using Eclipse.Domain.Exceptions;

namespace Eclipse.Domain.TodoItems;

internal class TodoItemAlreadyFinishedException : BusinessException
{
    public TodoItemAlreadyFinishedException(TodoItem item)
        : base($"TodoItem {item.Text} already finished") { }
}
