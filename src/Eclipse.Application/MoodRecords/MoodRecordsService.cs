using Eclipse.Application.Contracts.MoodRecords;
using Eclipse.Common.Clock;
using Eclipse.Common.Results;
using Eclipse.Domain.MoodRecords;
using Eclipse.Domain.Shared.Repositories;

namespace Eclipse.Application.MoodRecords;

internal sealed class MoodRecordsService : IMoodRecordsService
{
    private readonly IRepository<MoodRecord> _repository;

    private readonly ITimeProvider _timeProvider;

    public MoodRecordsService(IRepository<MoodRecord> repository, ITimeProvider timeProvider)
    {
        _repository = repository;
        _timeProvider = timeProvider;
    }

    public async Task<Result<MoodRecordDto>> CreateAsync(Guid userId, bool isGood, CancellationToken cancellationToken = default)
    {
        var record = new MoodRecord(Guid.NewGuid(), userId, isGood, _timeProvider.Now);

        await _repository.CreateAsync(record, cancellationToken);

        return new MoodRecordDto
        {
            Id = record.Id,
            CreatedAt = record.CreatedAt,
            IsGood = isGood,
            UserId = record.UserId,
        };
    }

    public async Task<List<MoodRecordDto>> GetListAsync(CancellationToken cancellationToken = default)
    {
        var records = await _repository.GetAllAsync(cancellationToken);

        return records.Select(r => new MoodRecordDto
        {
            Id = r.Id,
            CreatedAt = r.CreatedAt,
            IsGood = r.IsGood,
            UserId = r.UserId,
        }).ToList();
    }
}
