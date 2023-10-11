using Eclipse.Domain.Shared.Exceptions;

namespace Eclipse.Domain.Shared.TodoItems;

public class TodoItemAlreadyFinishedException : DomainException
{
    public TodoItemAlreadyFinishedException(string text)
        : base(TodoItemErrors.Messages.AlreadyFinished, text) { }
}
