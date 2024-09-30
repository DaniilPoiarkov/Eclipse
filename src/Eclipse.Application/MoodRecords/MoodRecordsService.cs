using Eclipse.Application.Contracts.MoodRecords;
using Eclipse.Common.Clock;
using Eclipse.Common.Results;
using Eclipse.Domain.MoodRecords;
using Eclipse.Domain.Shared.Errors;
using Eclipse.Domain.Shared.Repositories;
using Eclipse.Domain.Users;

namespace Eclipse.Application.MoodRecords;

internal sealed class MoodRecordsService : IMoodRecordsService
{
    private readonly IMoodRecordRepository _repository;

    private readonly UserManager _userManager;

    private readonly ITimeProvider _timeProvider;

    public MoodRecordsService(IMoodRecordRepository repository, UserManager userManager, ITimeProvider timeProvider)
    {
        _repository = repository;
        _userManager = userManager;
        _timeProvider = timeProvider;
    }

    public async Task<Result<MoodRecordDto>> CreateAsync(Guid userId, CreateMoodRecordDto model, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId, cancellationToken);

        if (user is null)
        {
            return DefaultErrors.EntityNotFound(typeof(User));
        }

        var record = user.CreateMoodRecord(model.State, _timeProvider.Now);

        await _repository.CreateAsync(record, cancellationToken);

        return new MoodRecordDto
        {
            Id = record.Id,
            CreatedAt = record.CreatedAt,
            State = record.State,
            UserId = record.UserId,
        };
    }

    public async Task<List<MoodRecordDto>> GetListAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var records = await _repository.GetByExpressionAsync(mr => mr.UserId == userId, cancellationToken);

        return records.Select(r => new MoodRecordDto
        {
            Id = r.Id,
            CreatedAt = r.CreatedAt,
            State = r.State,
            UserId = r.UserId,
        }).ToList();
    }
}
