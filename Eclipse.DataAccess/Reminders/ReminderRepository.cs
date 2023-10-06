using Eclipse.DataAccess.CosmosDb;
using Eclipse.DataAccess.EclipseCosmosDb;
using Eclipse.Domain.Reminders;

namespace Eclipse.DataAccess.Reminders;

internal class ReminderRepository : CosmosRepository<Reminder>, IReminderRepository
{
    // TODO: Throughout limit violation

    //public ReminderRepository(EclipseCosmosDbContext context) : base(context.Reminders)
    //{

    //}

    public ReminderRepository(IContainer<Reminder> container) : base(container)
    {
    }
}
