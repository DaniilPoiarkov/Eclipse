using Eclipse.Localization.Exceptions;

namespace Eclipse.Application.TodoItems.Exceptions;

public class TodoItemException : LocalizedException
{
    public TodoItemException(string message, params string[] args)
        : base(message, args) { }
}
