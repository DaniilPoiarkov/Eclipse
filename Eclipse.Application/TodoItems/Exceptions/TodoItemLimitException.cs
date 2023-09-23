using Eclipse.Localization.Exceptions;

namespace Eclipse.Application.TodoItems.Exceptions;

public class TodoItemLimitException : LocalizedException
{
    public TodoItemLimitException(int limit)
        : base("TodoItem:Limit", $"{limit}") { }
}
