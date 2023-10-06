using Eclipse.Domain.Reminders;
using Eclipse.Domain.Shared.Entities;
using Eclipse.Domain.Suggestions;
using Eclipse.Domain.TodoItems;

namespace Eclipse.DataAccess.InMemoryDb;

public class InMemoryDbContext : IDbContext
{
    public List<Suggestion> Suggestions { get; set; } = new();

    public List<TodoItem> TodoItems { get; set; } = new();

    public List<Reminder> Reminders { get; set; } = new();

    public virtual IList<TEntity> Set<TEntity>()
        where TEntity : Entity
    {
        if (Suggestions is List<TEntity> suggestions)
        {
            return suggestions;
        }

        if (TodoItems is List<TEntity> todoItems)
        {
            return todoItems;
        }

        if (Reminders is List<TEntity> reminders)
        {
            return reminders;
        }

        return new List<TEntity>();
    }

    public virtual void Update<TEntity>(TEntity entity)
        where TEntity : Entity
    {

    }
}
