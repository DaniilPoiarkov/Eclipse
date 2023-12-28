using Eclipse.Domain.Exceptions;
using Eclipse.Domain.Shared.TodoItems;

namespace Eclipse.Domain.TodoItems;

[Serializable]
internal sealed class TodoItemAlreadyFinishedException : DomainException
{
    internal TodoItemAlreadyFinishedException(string text)
        : base(TodoItemErrors.Messages.AlreadyFinished, text) { }
}
