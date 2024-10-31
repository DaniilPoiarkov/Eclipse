using Eclipse.Common.Results;

namespace Eclipse.Application.Contracts.MoodRecords;

public interface IMoodRecordsService
{
    Task<Result<MoodRecordDto>> CreateOrUpdateAsync(Guid userId, CreateMoodRecordDto model, CancellationToken cancellationToken = default);

    Task<List<MoodRecordDto>> GetListAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<Result<MoodRecordDto>> GetByIdAsync(Guid userId, Guid id, CancellationToken cancellationToken = default);
}
