using Eclipse.Application.Jobs;
using Eclipse.Domain.Users;
using Eclipse.Localization.Culture;

using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

using Quartz;

using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Eclipse.Application.Notifications.Donations;

internal sealed class RequestDonationsJob : UserProcessingJob, IJobWithKey
{
    private readonly ICurrentCulture _currentCulture;

    private readonly IStringLocalizer<RequestDonationsJob> _localizer;

    private readonly ITelegramBotClient _client;

    public static JobKey Key => new(nameof(RequestDonationsJob), "notifications");

    public RequestDonationsJob(
        ICurrentCulture currentCulture,
        IStringLocalizer<RequestDonationsJob> localizer,
        ITelegramBotClient client,
        IUserRepository userRepository,
        ILogger<RequestDonationsJob> logger) : base(userRepository, logger)
    {
        _currentCulture = currentCulture;
        _localizer = localizer;
        _client = client;
    }

    protected override async Task Execute(User user, CancellationToken cancellationToken)
    {
        using var _ = _currentCulture.UsingCulture(user.Culture);

        await _client.SendMessage(
            chatId: user.ChatId,
            text: _localizer["Notifications:Donations:Request"],
            replyMarkup: new InlineKeyboardMarkup(InlineKeyboardButton.WithUrl(
                text: _localizer["Notifications:Donations:ButtonText"],
                url: _localizer["Notifications:Donations:ButtonUrl"]
            )),
            parseMode: ParseMode.Html,
            cancellationToken: cancellationToken
        );
    }
}
