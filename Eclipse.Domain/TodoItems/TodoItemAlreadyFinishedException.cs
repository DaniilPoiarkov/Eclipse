using Eclipse.Domain.Exceptions;
using Eclipse.Domain.Shared.TodoItems;

namespace Eclipse.Domain.TodoItems;

internal class TodoItemAlreadyFinishedException : DomainException
{
    public TodoItemAlreadyFinishedException(TodoItem item)
        : base(TodoItemErrors.Messages.AlreadyFinished, item.Text) { }
}
