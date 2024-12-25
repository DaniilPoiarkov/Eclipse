using Eclipse.Application.Contracts.Telegram;
using Eclipse.Common.Events;
using Eclipse.Domain.Users.Events;

namespace Eclipse.Application.Users.EventHandlers;

internal sealed class TestEventHandler : 
    //INotificationHandler<TestDomainEvent>, 
    IEventHandler<TestDomainEvent>
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

internal sealed class AnotherTestEventHandler : IEventHandler<TestDomainEvent>
{
    private readonly ITelegramService _telegramService;

    public AnotherTestEventHandler(ITelegramService telegramService)
    {
        _telegramService = telegramService;
    }

    public Task Handle(TestDomainEvent @event, CancellationToken cancellationToken = default)
    {
        return _telegramService.Send(new SendMessageModel
        {
            ChatId = @event.ChatId,
            Message = $"Test event triggered #2"
        }, cancellationToken);
    }
}
