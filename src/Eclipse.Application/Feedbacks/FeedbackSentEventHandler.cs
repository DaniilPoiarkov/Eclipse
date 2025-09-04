using Eclipse.Application.Contracts.Telegram;
using Eclipse.Application.Contracts.Users;
using Eclipse.Common.Events;
using Eclipse.Common.Results;
using Eclipse.Domain.Feedbacks;

using Microsoft.Extensions.Options;

namespace Eclipse.Application.Feedbacks;

internal sealed class FeedbackSentEventHandler : IEventHandler<FeedbackSentEvent>
{
    private readonly ITelegramService _telegramService;

    private readonly IOptions<ApplicationOptions> _options;

    private readonly IUserService _userService;

    public FeedbackSentEventHandler(ITelegramService telegramService, IOptions<ApplicationOptions> options, IUserService userService)
    {
        _telegramService = telegramService;
        _options = options;
        _userService = userService;
    }

    public async Task Handle(FeedbackSentEvent @event, CancellationToken cancellationToken = default)
    {
        var result = await _userService.GetByIdAsync(@event.UserId, cancellationToken);

        var message = result.Match(
            user => $"Feedback from {user.Name}{user.UserName.FormattedOrEmpty(s => $", @{s}")}:{Environment.NewLine}Rate: {Enum.GetName(@event.Rate)}.{Environment.NewLine}{@event.Comment}",
            _ => $"Feedback from unknown user with id {@event.UserId}:{Environment.NewLine}Rate: {Enum.GetName(@event.Rate)}.{Environment.NewLine}{@event.Comment}"
        );

        var send = new SendMessageModel
        {
            ChatId = _options.Value.Chat,
            Message = message
        };

        await _telegramService.Send(send, cancellationToken);
    }
}
