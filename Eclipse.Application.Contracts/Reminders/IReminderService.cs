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
}
