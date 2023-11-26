using Eclipse.Domain.Shared.Exceptions;

namespace Eclipse.Domain.Shared.TodoItems;

public class TodoItemLimitException : DomainException
{
    public TodoItemLimitException(int limit)
        : base("TodoItem:Limit", $"{limit}") { }
}
