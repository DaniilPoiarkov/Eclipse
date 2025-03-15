using Eclipse.DataAccess.Cosmos;
using Eclipse.DataAccess.Repositories;
using Eclipse.Domain.OutboxMessages;

using Microsoft.EntityFrameworkCore;

namespace Eclipse.DataAccess.OutboxMessages;

internal sealed class OutboxMessageRepository : RepositoryBase<OutboxMessage>, IOutboxMessageRepository
{
    public OutboxMessageRepository(EclipseDbContext context)
        : base(context) { }

    public async Task DeleteSuccessfullyProcessedAsync(CancellationToken cancellationToken = default)
    {
        var processed = await DbSet.Where(new OutboxMessageSuccessfullyProcessedSpecification())
            .ToListAsync(cancellationToken);

        DbSet.RemoveRange(processed);

        await Context.SaveChangesAsync(cancellationToken);
    }

    public Task<List<OutboxMessage>> GetNotProcessedAsync(int count, CancellationToken cancellationToken = default)
    {
        return DbSet.Where(new OutboxMessageNotProcessedSpecification())
            .OrderBy(m => m.OccuredAt)
            .Take(count)
            .ToListAsync(cancellationToken);
    }
}
