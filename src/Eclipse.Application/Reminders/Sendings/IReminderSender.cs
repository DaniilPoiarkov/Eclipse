namespace Eclipse.Application.Reminders.Sendings;

public interface IReminderSender
{
    Task Send(ReminderArguments arguments, CancellationToken cancellationToken = default);
}
