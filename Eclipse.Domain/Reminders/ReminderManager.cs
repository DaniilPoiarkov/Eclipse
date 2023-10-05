using Eclipse.Domain.Exceptions;

namespace Eclipse.Domain.Reminders;

public class ReminderManager
{
    private readonly IReminderRepository _reminderRepository;

    public ReminderManager(IReminderRepository reminderRepository)
    {
        _reminderRepository = reminderRepository;
    }

    public async Task<Reminder> CreateAsync(Guid userId, string text, TimeOnly notifyAt, CancellationToken cancellationToken = default)
    {
        var reminder = new Reminder(Guid.NewGuid(), userId, text, notifyAt);

        return await _reminderRepository.CreateAsync(reminder, cancellationToken)
            ?? throw new EntityNotFoundException(typeof(Reminder));
    }

    public Task DeleteAsync(Guid reminderId, CancellationToken cancellationToken = default)
    {
        return _reminderRepository.DeleteAsync(reminderId, cancellationToken);
    }

    public Task<IReadOnlyList<Reminder>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return _reminderRepository.GetByExpressionAsync(n => n.UserId == userId, cancellationToken);
    }
}
