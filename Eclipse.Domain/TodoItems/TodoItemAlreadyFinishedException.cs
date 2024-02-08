using Eclipse.Domain.Exceptions;

namespace Eclipse.Domain.TodoItems;

[Serializable]
internal sealed class TodoItemAlreadyFinishedException : DomainException
{
    internal TodoItemAlreadyFinishedException(string text)
        : base("TodoItem:AlreadyFinished", text) { }
}
