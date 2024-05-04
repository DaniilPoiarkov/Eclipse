using Eclipse.Application.Contracts.IdentityUsers;
using Eclipse.Application.Contracts.Reminders;
using Eclipse.Application.IdentityUsers;
using Eclipse.Common.Results;
using Eclipse.Domain.IdentityUsers;
using Eclipse.Domain.Shared.Errors;

namespace Eclipse.Application.Reminders;

internal sealed class ReminderService : IReminderService
{
    private readonly IdentityUserManager _userManager;

    public ReminderService(IdentityUserManager userManager)
    {
        _userManager = userManager;
    }

    public async Task<Result<IdentityUserDto>> CreateReminderAsync(Guid userId, ReminderCreateDto createReminderDto, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId, cancellationToken);

        if (user is null)
        {
            return DefaultErrors.EntityNotFound(typeof(IdentityUser));
        }

        user.AddReminder(createReminderDto.Text, createReminderDto.NotifyAt);

        await _userManager.UpdateAsync(user, cancellationToken);

        return user.ToDto();
    }

    public async Task<Result<IdentityUserDto>> RemoveRemindersForTime(Guid userId, TimeOnly time, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId, cancellationToken);

        if (user is null)
        {
            return DefaultErrors.EntityNotFound(typeof(IdentityUser));
        }

        user.RemoveRemindersForTime(time);

        await _userManager.UpdateAsync(user, cancellationToken);

        return user.ToDto();
    }
}
