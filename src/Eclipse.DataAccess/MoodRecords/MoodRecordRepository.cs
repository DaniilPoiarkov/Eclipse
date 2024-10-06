using Eclipse.DataAccess.CosmosDb;
using Eclipse.DataAccess.Repositories;
using Eclipse.Domain.MoodRecords;

namespace Eclipse.DataAccess.MoodRecords;

internal sealed class MoodRecordRepository : RepositoryBase<MoodRecord>, IMoodRecordRepository
{
    public MoodRecordRepository(EclipseDbContext context)
        : base(context) { }
}
