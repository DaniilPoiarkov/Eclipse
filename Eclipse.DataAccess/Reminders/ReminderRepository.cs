using Eclipse.DataAccess.CosmosDb;
using Eclipse.DataAccess.EclipseCosmosDb;
using Eclipse.Domain.Reminders;

namespace Eclipse.DataAccess.Reminders;

internal class ReminderRepository : CosmosRepository<Reminder>, IReminderRepository
{
    public ReminderRepository(EclipseCosmosDbContext context) : base(context.Reminders)
    {

    }
}
