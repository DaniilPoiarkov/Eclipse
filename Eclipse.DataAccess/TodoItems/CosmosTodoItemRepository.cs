using Eclipse.DataAccess.CosmosDb;
using Eclipse.DataAccess.EclipseCosmosDb;
using Eclipse.Domain.TodoItems;

namespace Eclipse.DataAccess.TodoItems;

internal class CosmosTodoItemRepository : CosmosRepository<TodoItem>,ITodoItemRepository
{
    public CosmosTodoItemRepository(EclipseCosmosDbContext context)
        : base(context, $"{nameof(TodoItem)}s") { }
}
