using Eclipse.Application.Contracts.Users;
using Eclipse.Application.Contracts.Telegram;
using Eclipse.Common.Telegram;
using Eclipse.Domain.Suggestions;

using MediatR;

using Microsoft.Extensions.Options;

namespace Eclipse.Application.Suggestions;

public sealed class NewSuggestionSentEventHandler : INotificationHandler<NewSuggestionSentDomainEvent>
{
    private readonly ITelegramService _telegramService;

    private readonly IOptions<TelegramOptions> _options;

    private readonly IUserService _userService;

    public NewSuggestionSentEventHandler(ITelegramService telegramService, IOptions<TelegramOptions> options, IUserService userService)
    {
        _telegramService = telegramService;
        _options = options;
        _userService = userService;
    }

    public async Task Handle(NewSuggestionSentDomainEvent notification, CancellationToken cancellationToken)
    {
        var userResult = await _userService.GetByChatIdAsync(notification.ChatId, cancellationToken);

        var message = userResult.IsSuccess
            ? $"Suggestion from {userResult.Value.Name}{userResult.Value.UserName.FormattedOrEmpty(s => $", @{s}")}:{Environment.NewLine}{notification.Text}"
            : $"Suggestion from unknown user with chat id {notification.ChatId}:{Environment.NewLine}{notification.Text}";

        var send = new SendMessageModel
        {
            ChatId = _options.Value.Chat,
            Message = message
        };

        await _telegramService.Send(send, cancellationToken);
    }
}
