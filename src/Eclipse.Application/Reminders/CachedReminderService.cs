using Eclipse.Application.Caching;
using Eclipse.Application.Contracts.Reminders;
using Eclipse.Application.Contracts.Users;
using Eclipse.Common.Results;

namespace Eclipse.Application.Reminders;

internal sealed class CachedReminderService : UserCachingFixture, IReminderService
{
    private readonly IReminderService _reminderService;

    public CachedReminderService(IUserCache userCache, IReminderService reminderService) : base(userCache)
    {
        _reminderService = reminderService;
    }

    public Task<Result<UserDto>> CreateReminderAsync(Guid userId, ReminderCreateDto createReminderDto, CancellationToken cancellationToken = default)
    {
        return WithCachingAsync(() => _reminderService.CreateReminderAsync(userId, createReminderDto, cancellationToken), cancellationToken);
    }

    public Task<Result<UserDto>> RemoveRemindersForTime(Guid userId, TimeOnly time, CancellationToken cancellationToken = default)
    {
        return WithCachingAsync(() => _reminderService.RemoveRemindersForTime(userId, time, cancellationToken), cancellationToken);
    }
}
