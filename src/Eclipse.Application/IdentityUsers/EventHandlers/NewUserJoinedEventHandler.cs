using Eclipse.Application.Contracts.IdentityUsers;
using Eclipse.Application.Contracts.Telegram;
using Eclipse.Common.Telegram;
using Eclipse.Domain.IdentityUsers.Events;
using Eclipse.Localization.Localizers;

using MediatR;

using Microsoft.Extensions.Options;

namespace Eclipse.Application.IdentityUsers.EventHandlers;

public sealed class NewUserJoinedEventHandler : INotificationHandler<NewUserJoinedDomainEvent>
{
    private readonly ITelegramService _telegramService;

    private readonly ILocalizer _localizer;

    private readonly IOptions<TelegramOptions> _telegramOptions;

    private readonly IIdentityUserService _identityUserService;

    public NewUserJoinedEventHandler(
        ITelegramService telegramService,
        ILocalizer localizer,
        IOptions<TelegramOptions> telegramOptions,
        IIdentityUserService identityUserService)
    {
        _telegramService = telegramService;
        _localizer = localizer;
        _telegramOptions = telegramOptions;
        _identityUserService = identityUserService;
    }

    public async Task Handle(NewUserJoinedDomainEvent notification, CancellationToken cancellationToken)
    {
        var result = await _identityUserService.GetByChatIdAsync(_telegramOptions.Value.Chat, cancellationToken);

        if (!result.IsSuccess)
        {
            return;
        }

        var user = result.Value;
        var localizedString = _localizer["IdentityUser:Events:NewUserJoined", user.Culture];

        var content = string.Format(localizedString, notification.UserId, notification.UserName, notification.Name, notification.Surname);

        var message = new SendMessageModel
        {
            Message = content,
            ChatId = user.ChatId
        };

        await _telegramService.Send(message, cancellationToken);
    }
}
