using Eclipse.DataAccess.Cosmos;
using Eclipse.DataAccess.Repositories;
using Eclipse.Domain.MoodRecords;

using Microsoft.EntityFrameworkCore;

namespace Eclipse.DataAccess.MoodRecords;

internal sealed class MoodRecordRepository : RepositoryBase<MoodRecord>, IMoodRecordRepository
{
    public MoodRecordRepository(EclipseDbContext context)
        : base(context) { }

    public Task<MoodRecord?> FindForDateAsync(Guid userId, DateTime createdAt, CancellationToken cancellationToken = default)
    {
        return DbSet.Where(mr => mr.UserId == userId && mr.CreatedAt == createdAt)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
