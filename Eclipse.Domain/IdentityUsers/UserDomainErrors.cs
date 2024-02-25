using Eclipse.Common.Results;
using Eclipse.Domain.TodoItems;

namespace Eclipse.Domain.IdentityUsers;

internal static class UserDomainErrors
{
    internal static Error TodoItemNotFound() => Error.NotFound("IdentityUser.FinishTodoItem", "Entity:NotFound", nameof(TodoItem));
    internal static Error TodoItemsLimit(int limit) => Error.Validation("IdentityUser.AddTodoItem.Limit", "TodoItem:Limit", limit);
    internal static Error TodoItemIsEmpty() => Error.Validation("IdentityUser.AddTodoItem.Empty", "TodoItem:Empty");
    internal static Error TodoItemTooLong(int maxLength) => Error.Validation("IdentityUser.AddTodoItem.MaxLength", "TodoItem:MaxLength", maxLength);
    internal static Error DuplicateData(string param, object value) => Error.Validation("IdentityUser.Create", "IdentityUser:DuplicateData", param, value);
}
