using Eclipse.Common.Caching;
using Eclipse.DataAccess.Repositories;
using Eclipse.DataAccess.Repositories.Caching;
using Eclipse.Domain.MoodRecords;

namespace Eclipse.DataAccess.MoodRecords;

internal sealed class CachedMoodRecordRepository : CachedRepositoryBase<MoodRecord, IMoodRecordRepository>, IMoodRecordRepository
{
    public CachedMoodRecordRepository(IMoodRecordRepository repository, ICacheService cacheService, IExpressionHashStore expressionHashStore)
        : base(repository, cacheService, expressionHashStore) { }

    public Task<MoodRecord?> FindForDateAsync(Guid userId, DateTime createdAt, CancellationToken cancellationToken = default)
    {
        return Repository.FindForDateAsync(userId, createdAt, cancellationToken);
    }
}
