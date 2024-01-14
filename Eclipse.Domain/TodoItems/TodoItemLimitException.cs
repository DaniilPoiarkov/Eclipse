using Eclipse.Infrastructure.Exceptions;

namespace Eclipse.Domain.TodoItems;

[Serializable]
internal sealed class TodoItemLimitException : EclipseValidationException
{
    internal TodoItemLimitException(int limit)
        : base("TodoItem:Limit", $"{limit}") { }
}
