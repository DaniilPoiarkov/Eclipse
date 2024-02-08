using Eclipse.Application.Contracts.Telegram;
using Eclipse.Domain.IdentityUsers.Events;
using Eclipse.Infrastructure.Builder;

using MediatR;

using Microsoft.Extensions.Options;

namespace Eclipse.Application.IdentityUsers.EventHandlers;

public sealed class TestEventHandler : INotificationHandler<TestDomainEvent>
{
    private readonly ITelegramService _telegramService;

    private readonly IOptions<TelegramOptions> _telegramOptions;

    public TestEventHandler(ITelegramService telegramService, IOptions<TelegramOptions> telegramOptions)
    {
        _telegramService = telegramService;
        _telegramOptions = telegramOptions;
    }

    public async Task Handle(TestDomainEvent notification, CancellationToken cancellationToken)
    {
        await _telegramService.Send(new SendMessageModel
        {
            ChatId = _telegramOptions.Value.Chat,
            Message = $"Test event triggered"
        }, cancellationToken);
    }
}
