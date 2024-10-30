using Eclipse.Domain.Shared.Repositories;

namespace Eclipse.Domain.MoodRecords;

public interface IMoodRecordRepository : IRepository<MoodRecord>
{
    Task<MoodRecord?> FindForDateAsync(Guid userId, DateTime createdAt, CancellationToken cancellationToken);
}
