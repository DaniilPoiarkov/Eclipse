using Eclipse.Application.Contracts.Telegram;
using Eclipse.Common.Events;
using Eclipse.Domain.Users.Events;

namespace Eclipse.Application.Notifications.Test;

internal sealed class TestEventHandler : IEventHandler<TestDomainEvent>
{
    private readonly ITelegramService _telegramService;

    public TestEventHandler(ITelegramService telegramService)
    {
        _telegramService = telegramService;
    }

    public Task Handle(TestDomainEvent notification, CancellationToken cancellationToken)
    {
        return _telegramService.Send(new SendMessageModel
        {
            ChatId = notification.ChatId,
            Message = $"Test event triggered"
        }, cancellationToken);
    }
}
