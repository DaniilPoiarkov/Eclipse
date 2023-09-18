using Eclipse.DataAccess.DbContext;
using Eclipse.Domain.TodoItems;

namespace Eclipse.DataAccess.TodoItems;

internal class TodoItemRepository : Repository<TodoItem>, ITodoItemRepository
{
    public TodoItemRepository(IDbContext context) : base(context)
    {
    }
}
