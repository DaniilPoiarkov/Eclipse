using Eclipse.Application.Contracts.Telegram;
using Eclipse.Application.Contracts.Users;
using Eclipse.Common.Events;
using Eclipse.Domain.Users.Events;
using Eclipse.Localization.Culture;

using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace Eclipse.Application.Users.NewUserJoined;

public sealed class NewUserJoinedEventHandler : IEventHandler<NewUserJoinedDomainEvent>
{
    private readonly ITelegramService _telegramService;

    private readonly IStringLocalizer<NewUserJoinedEventHandler> _localizer;

    private readonly IOptions<ApplicationOptions> _telegramOptions;

    private readonly IUserService _userService;

    private readonly ICurrentCulture _currentCulture;

    public NewUserJoinedEventHandler(
        ITelegramService telegramService,
        IStringLocalizer<NewUserJoinedEventHandler> localizer,
        IOptions<ApplicationOptions> telegramOptions,
        IUserService userService,
        ICurrentCulture currentCulture)
    {
        _telegramService = telegramService;
        _localizer = localizer;
        _telegramOptions = telegramOptions;
        _userService = userService;
        _currentCulture = currentCulture;
    }

    public async Task Handle(NewUserJoinedDomainEvent notification, CancellationToken cancellationToken)
    {
        var result = await _userService.GetByChatIdAsync(_telegramOptions.Value.Chat, cancellationToken);

        if (!result.IsSuccess)
        {
            return;
        }

        var user = result.Value;

        using var _ = _currentCulture.UsingCulture(user.Culture);

        var content = _localizer["User:Events:NewUserJoined", notification.UserId, notification.UserName, notification.Name, notification.Surname];

        var message = new SendMessageModel
        {
            Message = content,
            ChatId = user.ChatId
        };

        await _telegramService.Send(message, cancellationToken);
    }
}
