using Eclipse.Domain.Exceptions;

namespace Eclipse.Domain.TodoItems;

internal class TodoItemAlreadyFinishedException : DomainException
{
    public TodoItemAlreadyFinishedException(TodoItem item)
        : base(TodoItemErrors.Messages.AlreadyFinished, item.Text) { }
}
