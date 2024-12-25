using Eclipse.DataAccess.CosmosDb;
using Eclipse.DataAccess.Repositories;
using Eclipse.Domain.InboxMessages;
using Eclipse.Domain.Shared.InboxMessages;

using Microsoft.EntityFrameworkCore;

namespace Eclipse.DataAccess.InboxMessages;

internal sealed class InboxMessageRepository : RepositoryBase<InboxMessage>, IInboxMessageRepository
{
    public InboxMessageRepository(EclipseDbContext context)
        : base(context) { }

    public Task DeleteSuccessfullyProcessedAsync(CancellationToken cancellationToken = default)
    {
        return DbSet.Where(i => i.Status == InboxMessageStatus.Processed)
            .ExecuteDeleteAsync(cancellationToken);
    }

    public Task<List<InboxMessage>> GetPendingAsync(int count, CancellationToken cancellationToken = default)
    {
        return DbSet.Where(i => i.Status == InboxMessageStatus.Pending)
            .OrderBy(i => i.OccuredAt)
            .Take(count)
            .ToListAsync(cancellationToken);
    }
}
