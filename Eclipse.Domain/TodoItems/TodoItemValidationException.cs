using Eclipse.Domain.Exceptions;

namespace Eclipse.Domain.TodoItems;

[Serializable]
internal sealed class TodoItemValidationException : DomainException
{
    internal TodoItemValidationException(params string[] args)
        : base("Eclipse:ValidationFailed", args) { }
}
