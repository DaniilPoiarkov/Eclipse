using Eclipse.Application.Caching;
using Eclipse.Application.Contracts.IdentityUsers;
using Eclipse.Application.Contracts.Reminders;

namespace Eclipse.Application.Reminders;

internal class CachedReminderService : IdentityUserCachingFixture, IReminderService
{
    private readonly IReminderService _reminderService;

    public CachedReminderService(IIdentityUserCache userCache, IReminderService reminderService) : base(userCache)
    {
        _reminderService = reminderService;
    }

    public Task<IdentityUserDto> CreateReminderAsync(Guid userId, ReminderCreateDto createReminderDto, CancellationToken cancellationToken = default)
    {
        return WithCachingAsync(() => _reminderService.CreateReminderAsync(userId, createReminderDto, cancellationToken));
    }

    public Task<IdentityUserDto> RemoveRemindersForTime(Guid userId, TimeOnly time, CancellationToken cancellationToken = default)
    {
        return WithCachingAsync(() => _reminderService.RemoveRemindersForTime(userId, time, cancellationToken));
    }
}
