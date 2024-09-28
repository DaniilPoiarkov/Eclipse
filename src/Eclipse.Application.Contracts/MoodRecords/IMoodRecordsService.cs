using Eclipse.Common.Results;

namespace Eclipse.Application.Contracts.MoodRecords;

public interface IMoodRecordsService
{
    Task<Result<MoodRecordDto>> CreateAsync(Guid userId, CreateMoodRecordDto model, CancellationToken cancellationToken = default);

    Task<List<MoodRecordDto>> GetListAsync(Guid userId, CancellationToken cancellationToken = default);
}
