using Eclipse.DataAccess.InMemoryDb;
using Eclipse.Domain.Reminders;

namespace Eclipse.DataAccess.Reminders;

internal class InMemoryRemiderRepository : InMemoryRepository<Reminder>, IReminderRepository
{
    public InMemoryRemiderRepository(IDbContext context) : base(context)
    {
    }
}
