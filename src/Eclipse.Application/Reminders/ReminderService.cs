using Eclipse.Application.Contracts.Reminders;
using Eclipse.Application.Contracts.Users;
using Eclipse.Application.Users;
using Eclipse.Common.Results;
using Eclipse.Domain.Reminders;
using Eclipse.Domain.Shared.Errors;
using Eclipse.Domain.Users;

namespace Eclipse.Application.Reminders;

internal sealed class ReminderService : IReminderService
{
    private readonly UserManager _userManager;

    public ReminderService(UserManager userManager)
    {
        _userManager = userManager;
    }

    public async Task<Result<UserDto>> CreateAsync(Guid userId, ReminderCreateDto createReminderDto, CancellationToken cancellationToken = default)
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

    public async Task<Result<ReminderDto>> GetAsync(Guid userId, Guid reminderId, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId, cancellationToken);

        if (user is null)
        {
            return DefaultErrors.EntityNotFound(typeof(User));
        }

        var reminder = user.GetReminder(reminderId);

        if (reminder is null)
        {
            return DefaultErrors.EntityNotFound(typeof(Reminder));
        }

        return reminder.ToDto();
    }

    public async Task<Result<List<ReminderDto>>> GetListAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId, cancellationToken);

        if (user is null)
        {
            return DefaultErrors.EntityNotFound(typeof(User));
        }

        return user.Reminders.Select(reminder => reminder.ToDto()).ToList();
    }

    public async Task<Result<UserDto>> RemoveForTimeAsync(Guid userId, TimeOnly time, CancellationToken cancellationToken = default)
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
