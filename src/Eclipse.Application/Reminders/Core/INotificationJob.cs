namespace Eclipse.Application.Reminders.Core;

internal interface INotificationJob<TArgs>
{
    Task Handle(TArgs args, CancellationToken cancellationToken = default);
}
