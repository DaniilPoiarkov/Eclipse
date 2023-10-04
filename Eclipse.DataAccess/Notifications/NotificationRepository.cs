using Eclipse.DataAccess.CosmosDb;
using Eclipse.DataAccess.EclipseCosmosDb;
using Eclipse.Domain.Notifications;

namespace Eclipse.DataAccess.Notifications;

internal class NotificationRepository : CosmosRepository<Notification>, INotificationRepository
{
    public NotificationRepository(EclipseCosmosDbContext context) : base(context.Notifications)
    {

    }
}
