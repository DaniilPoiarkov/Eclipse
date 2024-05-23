using Eclipse.Common.Results;

namespace Eclipse.Domain.TodoItems;

internal static class TodoItemDomainErrors
{
    internal static Error TodoItemIsEmpty() => Error.Validation("User.AddTodoItem.Empty", "TodoItem:Empty");
    internal static Error TodoItemTooLong(int maxLength) => Error.Validation("User.AddTodoItem.MaxLength", "TodoItem:MaxLength", maxLength);
    internal static Error AlreadyFinished(string text) => Error.Conflict("TodoItems.Finish", "TodoItem:AlreadyFinished", text);
}
