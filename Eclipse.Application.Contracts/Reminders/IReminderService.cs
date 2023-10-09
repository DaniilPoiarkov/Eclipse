using Eclipse.Application.Contracts.IdentityUsers;

namespace Eclipse.Application.Contracts.Reminders;

public interface IReminderService
{
    /// <summary>
    /// Creates reminder for specified user
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="createReminderDto"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IdentityUserDto> CreateReminderAsync(Guid userId, ReminderCreateDto createReminderDto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes all user reminders for specified time
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="time"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IdentityUserDto> RemoveRemindersForTime(Guid userId, TimeOnly time, CancellationToken cancellationToken = default);
}
