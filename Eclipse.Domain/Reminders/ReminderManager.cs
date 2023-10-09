using Eclipse.Domain.Exceptions;
using Eclipse.Domain.IdentityUsers;

namespace Eclipse.Domain.Reminders;

public class ReminderManager
{
    private readonly IdentityUserManager _manager;

    public ReminderManager(IdentityUserManager manager)
    {
        _manager = manager;
    }

    /// <summary>
    /// Removes all user reminders for specified time
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="time"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="EntityNotFoundException"></exception>
    public async Task RemoveRemindersForTime(Guid userId, TimeOnly time, CancellationToken cancellationToken = default)
    {
        var user = await _manager.FindByIdAsync(userId, cancellationToken)
            ?? throw new EntityNotFoundException(typeof(IdentityUser));

        var specification = new ReminderNotifyAtSpecification(time);

        var reminders = user.Reminders.Where(specification);

        user.RemoveReminders(reminders);

        await _manager.UpdateAsync(user, cancellationToken);
    }
}
