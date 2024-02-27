using Eclipse.Common.Results;

namespace Eclipse.Domain.TodoItems;

internal static class TodoItemDomainErrors
{
    internal static Error TodoItemIsEmpty() => Error.Validation("IdentityUser.AddTodoItem.Empty", "TodoItem:Empty");
    internal static Error TodoItemTooLong(int maxLength) => Error.Validation("IdentityUser.AddTodoItem.MaxLength", "TodoItem:MaxLength", maxLength);
    internal static Error AlreadyFinished(string text) => Error.Conflict("TodoItems.Finish", "TodoItem:AlreadyFinished", text);
}
