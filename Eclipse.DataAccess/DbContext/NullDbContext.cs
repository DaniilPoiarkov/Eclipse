using Eclipse.Domain.Suggestions;
using Eclipse.Domain.TodoItems;

namespace Eclipse.DataAccess.DbContext;

internal class NullDbContext
{
    public List<Suggestion> Suggestions { get; set; } = new();

    public List<TodoItem> TodoItems { get; set; } = new();
}
