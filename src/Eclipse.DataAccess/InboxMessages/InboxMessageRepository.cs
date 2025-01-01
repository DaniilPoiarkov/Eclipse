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

    public async Task DeleteSuccessfullyProcessedAsync(CancellationToken cancellationToken = default)
    {
        var messages = await DbSet.Where(i => i.Status == InboxMessageStatus.Processed)
            .ToListAsync(cancellationToken);

        DbSet.RemoveRange(messages);

        await Context.SaveChangesAsync(cancellationToken);
    }

    public Task<List<InboxMessage>> GetPendingAsync(int count, CancellationToken cancellationToken = default)
    {
        return DbSet.Where(i => i.Status == InboxMessageStatus.Pending)
            .OrderBy(i => i.OccuredAt)
            .Take(count)
            .ToListAsync(cancellationToken);
    }
}
