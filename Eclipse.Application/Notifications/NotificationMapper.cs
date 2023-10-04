using Eclipse.Application.Contracts.Base;
using Eclipse.Application.Contracts.Notifications;
using Eclipse.Domain.Notifications;

namespace Eclipse.Application.Notifications;

public class NotificationMapper : IMapper<Notification, NotificationDto>
{
    public NotificationDto Map(Notification value)
    {
        return new NotificationDto
        {
            Id = value.Id,
            Name = value.Name,
            NotifyAt = value.NotifyAt,
            UserChatId = value.UserChatId,
            UserId = value.UserId,
        };
    }
}
