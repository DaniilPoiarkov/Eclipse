using Eclipse.Application.Contracts.Telegram;
using Eclipse.Application.Contracts.Users;
using Eclipse.Common.Telegram;
using Eclipse.Domain.Users.Events;
using Eclipse.Localization;
using MediatR;

using Microsoft.Extensions.Options;

namespace Eclipse.Application.Users.EventHandlers;

public sealed class NewUserJoinedEventHandler : INotificationHandler<NewUserJoinedDomainEvent>
{
    private readonly ITelegramService _telegramService;

    private readonly ILocalizer _localizer;

    private readonly IOptions<TelegramOptions> _telegramOptions;

    private readonly IUserService _userService;

    public NewUserJoinedEventHandler(
        ITelegramService telegramService,
        ILocalizer localizer,
        IOptions<TelegramOptions> telegramOptions,
        IUserService userService)
    {
        _telegramService = telegramService;
        _localizer = localizer;
        _telegramOptions = telegramOptions;
        _userService = userService;
    }

    public async Task Handle(NewUserJoinedDomainEvent notification, CancellationToken cancellationToken)
    {
        var result = await _userService.GetByChatIdAsync(_telegramOptions.Value.Chat, cancellationToken);

        if (!result.IsSuccess)
        {
            return;
        }

        var user = result.Value;
        var localizedString = _localizer["User:Events:NewUserJoined", user.Culture];

        var content = string.Format(localizedString, notification.UserId, notification.UserName, notification.Name, notification.Surname);

        var message = new SendMessageModel
        {
            Message = content,
            ChatId = user.ChatId
        };

        await _telegramService.Send(message, cancellationToken);
    }
}
