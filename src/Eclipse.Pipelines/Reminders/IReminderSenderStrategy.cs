using Eclipse.Application.Reminders.Sendings;

namespace Eclipse.Pipelines.Reminders;

internal interface IReminderSenderStrategy
{
    Task Send(ReminderArguments arguments, CancellationToken cancellationToken = default);
}
