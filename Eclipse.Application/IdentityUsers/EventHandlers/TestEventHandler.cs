﻿using Eclipse.Application.Contracts.Telegram;
using Eclipse.Domain.IdentityUsers.Events;

using MediatR;

namespace Eclipse.Application.IdentityUsers.EventHandlers;

public sealed class TestEventHandler : INotificationHandler<TestDomainEvent>
{
    private readonly ITelegramService _telegramService;

    public TestEventHandler(ITelegramService telegramService)
    {
        _telegramService = telegramService;
    }

    public async Task Handle(TestDomainEvent notification, CancellationToken cancellationToken)
    {
        await _telegramService.Send(new SendMessageModel
        {
            ChatId = notification.ChatId,
            Message = $"Test event triggered"
        }, cancellationToken);
    }
}
