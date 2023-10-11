using Eclipse.Domain.Shared.Exceptions;

namespace Eclipse.Domain.Shared.TodoItems;

public class TodoItemValidationException : DomainException
{
    public TodoItemValidationException(params string[] args)
        : base("Eclipse:ValidationFailed", args) { }
}
