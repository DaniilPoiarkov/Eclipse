using Eclipse.Common.Results;

namespace Eclipse.Application.Contracts.MoodRecords;

public interface IMoodRecordsService
{
    Task<Result<MoodRecordDto>> CreateAsync(Guid userId, bool isGood, CancellationToken cancellationToken = default);

    Task<List<MoodRecordDto>> GetListAsync(CancellationToken cancellationToken = default);
}
