using Eclipse.DataAccess.InMemoryDb;
using Eclipse.Domain.TodoItems;

namespace Eclipse.DataAccess.TodoItems;

internal class InMemoryTodoItemRepository : InMemoryRepository<TodoItem>, ITodoItemRepository
{
    public InMemoryTodoItemRepository(IDbContext context) : base(context)
    {
    }
}
