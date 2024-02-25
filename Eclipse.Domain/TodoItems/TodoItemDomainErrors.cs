using Eclipse.Common.Results;

namespace Eclipse.Domain.TodoItems;

internal static class TodoItemDomainErrors
{
    internal static Error AlreadyFinished(string text) => Error.Conflict("TodoItems.Finish", "TodoItem:AlreadyFinished", text);
}
