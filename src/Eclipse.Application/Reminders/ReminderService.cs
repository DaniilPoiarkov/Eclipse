using Eclipse.Application.Contracts.Users;
using Eclipse.Application.Contracts.Reminders;
using Eclipse.Application.Users;
using Eclipse.Common.Results;
using Eclipse.Domain.Users;
using Eclipse.Domain.Shared.Errors;

namespace Eclipse.Application.Reminders;

internal sealed class ReminderService : IReminderService
{
    private readonly UserManager _userManager;

    public ReminderService(UserManager userManager)
    {
        _userManager = userManager;
    }

    public async Task<Result<UserDto>> CreateReminderAsync(Guid userId, ReminderCreateDto createReminderDto, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId, cancellationToken);

        if (user is null)
        {
            return DefaultErrors.EntityNotFound(typeof(User));
        }

        user.AddReminder(createReminderDto.Text, createReminderDto.NotifyAt);

        await _userManager.UpdateAsync(user, cancellationToken);

        return user.ToDto();
    }

    public async Task<Result<UserDto>> RemoveRemindersForTime(Guid userId, TimeOnly time, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId, cancellationToken);

        if (user is null)
        {
            return DefaultErrors.EntityNotFound(typeof(User));
        }

        user.RemoveRemindersForTime(time);

        await _userManager.UpdateAsync(user, cancellationToken);

        return user.ToDto();
    }
}
