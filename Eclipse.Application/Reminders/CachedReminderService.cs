using Eclipse.Application.Contracts.IdentityUsers;
using Eclipse.Application.Contracts.Reminders;

namespace Eclipse.Application.Reminders;

internal class CachedReminderService : IReminderService
{
    private readonly IIdentityUserCache _userCache;

    private readonly IReminderService _reminderService;

    public CachedReminderService(IIdentityUserCache userCache, IReminderService reminderService)
    {
        _userCache = userCache;
        _reminderService = reminderService;
    }

    public async Task<IdentityUserDto> CreateReminderAsync(Guid userId, ReminderCreateDto createReminderDto, CancellationToken cancellationToken = default)
    {
        var user = await _reminderService.CreateReminderAsync(userId, createReminderDto, cancellationToken);

        _userCache.AddOrUpdate(user);

        return user;
    }

    public async Task<IdentityUserDto> RemoveRemindersForTime(Guid userId, TimeOnly time, CancellationToken cancellationToken = default)
    {
        var user = await _reminderService.RemoveRemindersForTime(userId, time, cancellationToken);

        _userCache.AddOrUpdate(user);

        return user;
    }
}
