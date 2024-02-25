using Eclipse.Common.Results;
using Eclipse.Domain.TodoItems;

namespace Eclipse.Domain.IdentityUsers;

internal static class UserDomainErrors
{
    internal static Error TodoItemNotFound() => Error.NotFound("IdentityUser.FinishTodoItem", "Entity:NotFound", nameof(TodoItem));
    internal static Error TodoItemsLimit(int limit) => Error.Validation("IdentityUser.AddTodoItem.Limit", "TodoItem:Limit", limit);
    internal static Error DuplicateData(string param, object value) => Error.Validation("IdentityUser.Create", "IdentityUser:DuplicateData", param, value);
}
