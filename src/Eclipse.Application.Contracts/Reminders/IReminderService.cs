using Eclipse.Application.Contracts.Users;
using Eclipse.Common.Results;

namespace Eclipse.Application.Contracts.Reminders;

public interface IReminderService
{
    Task<Result<ReminderDto>> CreateAsync(Guid userId, ReminderCreateDto model, CancellationToken cancellationToken = default);

    Task<Result<UserDto>> CreateAsync(long chatId, ReminderCreateDto model, CancellationToken cancellationToken = default);

    Task<Result> DeleteAsync(Guid userId, Guid reminderId, CancellationToken cancellationToken = default);

    Task<Result<List<ReminderDto>>> GetListAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<Result<ReminderDto>> GetAsync(Guid userId, Guid reminderId, CancellationToken cancellationToken = default);

    Task<Result<ReminderDto>> RescheduleAsync(Guid userId, Guid reminderId, DateTime notifyAt, CancellationToken cancellationToken = default);
}
