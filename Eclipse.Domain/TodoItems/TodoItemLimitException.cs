using Eclipse.Domain.Exceptions;

namespace Eclipse.Domain.TodoItems;

[Serializable]
internal sealed class TodoItemLimitException : DomainException
{
    internal TodoItemLimitException(int limit)
        : base("TodoItem:Limit", $"{limit}") { }
}
