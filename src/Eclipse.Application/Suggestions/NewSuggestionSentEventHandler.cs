﻿using Eclipse.Application.Contracts.Telegram;
using Eclipse.Application.Contracts.Users;
using Eclipse.Common.Events;
using Eclipse.Common.Results;
using Eclipse.Domain.Suggestions;

using Microsoft.Extensions.Options;

namespace Eclipse.Application.Suggestions;

public sealed class NewSuggestionSentEventHandler : IEventHandler<NewSuggestionSentDomainEvent>
{
    private readonly ITelegramService _telegramService;

    private readonly IOptions<ApplicationOptions> _options;

    private readonly IUserService _userService;

    public NewSuggestionSentEventHandler(ITelegramService telegramService, IOptions<ApplicationOptions> options, IUserService userService)
    {
        _telegramService = telegramService;
        _options = options;
        _userService = userService;
    }

    public async Task Handle(NewSuggestionSentDomainEvent notification, CancellationToken cancellationToken = default)
    {
        var result = await _userService.GetByChatIdAsync(notification.ChatId, cancellationToken);

        var message = result.Match(
            user => $"Suggestion from {user.Name}{user.UserName.FormattedOrEmpty(s => $", @{s}")}:{Environment.NewLine}{notification.Text}",
            _ => $"Suggestion from unknown user with chat id {notification.ChatId}:{Environment.NewLine}{notification.Text}"
        );

        var send = new SendMessageModel
        {
            ChatId = _options.Value.Chat,
            Message = message
        };

        await _telegramService.Send(send, cancellationToken);
    }
}
