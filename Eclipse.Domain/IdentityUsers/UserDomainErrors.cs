using Eclipse.Common.Results;
using Eclipse.Domain.TodoItems;

namespace Eclipse.Domain.IdentityUsers;

internal static class UserDomainErrors
{
    internal static Error TodoItemNotFound() => Error.NotFound("IdentityUser.FinishTodoItem", "Entity:NotFound", nameof(TodoItem));
}
