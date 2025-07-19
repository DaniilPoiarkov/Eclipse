using Eclipse.Application.Contracts.MoodRecords;
using Eclipse.Common.Clock;
using Eclipse.Common.Results;
using Eclipse.Domain.MoodRecords;
using Eclipse.Domain.Shared.Errors;
using Eclipse.Domain.Users;

namespace Eclipse.Application.MoodRecords;

internal sealed class MoodRecordsService : IMoodRecordsService
{
    private readonly IMoodRecordRepository _repository;

    private readonly IUserRepository _userRepository;

    private readonly ITimeProvider _timeProvider;

    public MoodRecordsService(IMoodRecordRepository repository, IUserRepository userRepository, ITimeProvider timeProvider)
    {
        _repository = repository;
        _userRepository = userRepository;
        _timeProvider = timeProvider;
    }

    public async Task<Result<MoodRecordDto>> CreateOrUpdateAsync(Guid userId, CreateMoodRecordDto model, CancellationToken cancellationToken = default)
    {
        var time = _timeProvider.Now.WithTime(0, 0);

        var existing = await _repository.FindForDateAsync(userId, time, cancellationToken);

        if (existing is not null)
        {
            return await UpdateAsync(model, existing, cancellationToken);
        }

        return await CreateAsync(userId, model, cancellationToken);
    }

    private async Task<Result<MoodRecordDto>> UpdateAsync(CreateMoodRecordDto model, MoodRecord existing, CancellationToken cancellationToken)
    {
        existing.SetState(model.State);
        return (await _repository.UpdateAsync(existing, cancellationToken)).ToDto();
    }

    private async Task<Result<MoodRecordDto>> CreateAsync(Guid userId, CreateMoodRecordDto model, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.FindAsync(userId, cancellationToken);

        if (user is null)
        {
            return DefaultErrors.EntityNotFound<User>();
        }

        var record = user.CreateMoodRecord(model.State, _timeProvider.Now.WithTime(0, 0));

        await _repository.CreateAsync(record, cancellationToken);

        return record.ToDto();
    }

    public async Task<Result<MoodRecordDto>> GetByIdAsync(Guid userId, Guid id, CancellationToken cancellationToken = default)
    {
        var record = await _repository.FindAsync(id, cancellationToken);

        if (record is null || record.UserId != userId)
        {
            return DefaultErrors.EntityNotFound<MoodRecord>();
        }

        return record.ToDto();
    }

    public async Task<List<MoodRecordDto>> GetListAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var records = await _repository.GetByExpressionAsync(mr => mr.UserId == userId, cancellationToken);

        return [.. records.Select(r => r.ToDto())];
    }
}
