using Eclipse.Application.Contracts.Telegram;
using Eclipse.Common.Events;
using Eclipse.Domain.Users.Events;

namespace Eclipse.Application.Users.Test;

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
