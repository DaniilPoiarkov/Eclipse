using Eclipse.Application.Caching;
using Eclipse.Application.Contracts.IdentityUsers;
using Eclipse.Application.Contracts.Reminders;
using Eclipse.Common.Results;

namespace Eclipse.Application.Reminders;

internal sealed class CachedReminderService : IdentityUserCachingFixture, IReminderService
{
    private readonly IReminderService _reminderService;

    public CachedReminderService(IIdentityUserCache userCache, IReminderService reminderService) : base(userCache)
    {
        _reminderService = reminderService;
    }

    public Task<Result<IdentityUserDto>> CreateReminderAsync(Guid userId, ReminderCreateDto createReminderDto, CancellationToken cancellationToken = default)
    {
        return WithCachingAsync(() => _reminderService.CreateReminderAsync(userId, createReminderDto, cancellationToken), cancellationToken);
    }

    public Task<Result<IdentityUserDto>> RemoveRemindersForTime(Guid userId, TimeOnly time, CancellationToken cancellationToken = default)
    {
        return WithCachingAsync(() => _reminderService.RemoveRemindersForTime(userId, time, cancellationToken), cancellationToken);
    }
}
