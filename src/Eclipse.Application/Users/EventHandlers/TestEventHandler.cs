using Eclipse.Application.Contracts.Telegram;
using Eclipse.Domain.Users.Events;

using MediatR;

namespace Eclipse.Application.Users.EventHandlers;

public sealed class TestEventHandler : INotificationHandler<TestDomainEvent>
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
