using Eclipse.Application.Contracts.Users;
using Eclipse.Application.Feedbacks.Collection;
using Eclipse.Localization.Culture;

using Microsoft.Extensions.Localization;

using Telegram.Bot;

namespace Eclipse.Pipelines.Jobs;

internal sealed class FeedbackCollector : IFeedbackCollector
{
    private readonly ITelegramBotClient _botClient;

    private readonly IStringLocalizer<FeedbackCollector> _localizer;

    private readonly IUserService _userService;

    private readonly ICurrentCulture _currentCulture;

    public FeedbackCollector(
        ITelegramBotClient botClient,
        IStringLocalizer<FeedbackCollector> localizer,
        IUserService userService,
        ICurrentCulture currentCulture)
    {
        _botClient = botClient;
        _localizer = localizer;
        _userService = userService;
        _currentCulture = currentCulture;
    }

    public async Task CollectAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var result = await _userService.GetByIdAsync(userId, cancellationToken);

        if (!result.IsSuccess)
        {
            return;
        }

        var user = result.Value;

        using var _ = _currentCulture.UsingCulture(user.Culture);

        var message = _localizer["Jobs:Feedback"];

        var sentMessage = await _botClient.SendMessage(
            chatId: user.ChatId,
            text: message,
            cancellationToken: cancellationToken
        );
    }
}
