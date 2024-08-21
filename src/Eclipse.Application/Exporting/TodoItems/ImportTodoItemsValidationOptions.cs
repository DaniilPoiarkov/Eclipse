using Eclipse.Domain.Users;

namespace Eclipse.Application.Exporting.TodoItems;

public sealed class ImportTodoItemsValidationOptions
{
    public List<User> Users { get; set; } = [];
}
