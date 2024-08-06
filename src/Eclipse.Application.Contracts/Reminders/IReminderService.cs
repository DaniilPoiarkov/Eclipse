using Eclipse.Application.Contracts.Users;
using Eclipse.Common.Results;

namespace Eclipse.Application.Contracts.Reminders;

public interface IReminderService
{
    /// <summary>
    /// Creates reminder for specified user
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="model"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Result<ReminderDto>> CreateAsync(Guid userId, ReminderCreateDto model, CancellationToken cancellationToken = default);

    Task<Result<UserDto>> CreateAsync(long chatId, ReminderCreateDto model, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes all user reminders for specified time
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="time"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Result<UserDto>> RemoveForTimeAsync(Guid userId, TimeOnly time, CancellationToken cancellationToken = default);

    Task<Result<List<ReminderDto>>> GetListAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<Result<ReminderDto>> GetAsync(Guid userId, Guid reminderId, CancellationToken cancellationToken = default);
}
