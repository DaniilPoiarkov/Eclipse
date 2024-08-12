using Eclipse.Common.Results;
using Eclipse.Domain.Shared.Errors;
using Eclipse.Domain.TodoItems;

namespace Eclipse.Domain.Users;

internal static class UserDomainErrors
{
    internal static Error TodoItemNotFound() => DefaultErrors.EntityNotFound(typeof(TodoItem));
    internal static Error TodoItemsLimit(int limit) => Error.Validation("User.AddTodoItem.Limit", "TodoItem:Limit", limit);
    internal static Error DuplicateTodoItem(Guid id) => Error.Validation("User.AddTodoItem.Duplication", "User:AddTodoItem:DuplicateTodoItem", id);
    internal static Error DuplicateData(string param, object value) => Error.Validation("Users.Create", "User:DuplicateData", param, value);
}
