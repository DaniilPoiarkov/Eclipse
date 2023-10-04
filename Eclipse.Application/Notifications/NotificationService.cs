using Eclipse.Application.Contracts.Base;
using Eclipse.Application.Contracts.Notifications;
using Eclipse.Application.Exceptions;
using Eclipse.Domain.Notifications;

namespace Eclipse.Application.Notifications;

internal class NotificationService : INotificationService
{
    private readonly NotificationManager _notificationManager;

    private readonly INotificationRepository _notificationRepository;

    private readonly IMapper<Notification, NotificationDto> _mapper;

    public NotificationService(NotificationManager notificationManager, INotificationRepository notificationRepository, IMapper<Notification, NotificationDto> mapper)
    {
        _notificationManager = notificationManager;
        _notificationRepository = notificationRepository;
        _mapper = mapper;
    }

    public async Task<NotificationDto> CreateAsync(NotificationCreateDto createDto, CancellationToken cancellationToken = default)
    {
        var notification = await _notificationManager.CreateAsync(createDto.UserId, createDto.UserChatId, createDto.Name, createDto.NotifyAt, cancellationToken)
            ?? throw new ObjectNotFoundException(nameof(Notification));

        return _mapper.Map(notification);
    }

    public Task DeleteAsync(Guid notificationId, CancellationToken cancellationToken = default)
    {
        return _notificationManager.DeleteAsync(notificationId, cancellationToken);
    }

    public async Task<IReadOnlyList<NotificationDto>> GetNotificationsByUserChatIdAsync(long chatId, CancellationToken cancellationToken = default)
    {
        var notifications = await _notificationManager.GetByUserChatIdAsync(chatId, cancellationToken)
            ?? throw new ObjectNotFoundException(nameof(Notification));

        return notifications
            .Select(_mapper.Map)
            .ToList();
    }
}
