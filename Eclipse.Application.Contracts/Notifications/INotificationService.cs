namespace Eclipse.Application.Contracts.Notifications;

public interface INotificationService
{
    Task<IReadOnlyList<NotificationDto>> GetNotificationsByUserChatIdAsync(long chatId, CancellationToken cancellationToken = default);

    Task<NotificationDto> CreateAsync(NotificationCreateDto createDto, CancellationToken cancellationToken = default);

    Task DeleteAsync(Guid notificationId, CancellationToken cancellationToken = default);
}
