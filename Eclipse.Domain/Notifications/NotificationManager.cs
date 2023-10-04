using Eclipse.Domain.Exceptions;

namespace Eclipse.Domain.Notifications;

public class NotificationManager
{
    private readonly INotificationRepository _notificationRepository;

    public NotificationManager(INotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    public async Task<Notification> CreateAsync(Guid userId, long userChatId, string name, DateTime notifyAt, CancellationToken cancellationToken = default)
    {
        var notification = new Notification(Guid.NewGuid(), userId, userChatId, name, notifyAt);

        return await _notificationRepository.CreateAsync(notification, cancellationToken)
            ?? throw new EntityNotFoundException(typeof(Notification));
    }

    public Task DeleteAsync(Guid notificationId, CancellationToken cancellationToken = default)
    {
        return _notificationRepository.DeleteAsync(notificationId, cancellationToken);
    }

    public Task<IReadOnlyList<Notification>> GetByUserChatIdAsync(long userChatId, CancellationToken cancellationToken = default)
    {
        return _notificationRepository.GetByExpressionAsync(n => n.UserChatId == userChatId, cancellationToken);
    }
}
