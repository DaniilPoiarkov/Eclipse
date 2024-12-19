using Eclipse.Application.Contracts.Users;
using Eclipse.Common.Clock;
using Eclipse.Localization.Extensions;
using Eclipse.Pipelines.Jobs.Evening;

using Microsoft.Extensions.Localization;

using Quartz;

using Telegram.Bot;

namespace Eclipse.Pipelines.Jobs.Morning;

internal sealed class SendGoodMorningJob : EclipseJobBase
{
    private static readonly TimeOnly _morning = new(9, 0);

    private readonly ITelegramBotClient _botClient;

    private readonly IStringLocalizer<CollectMoodRecordsJob> _localizer;

    private readonly IUserService _userService;

    private readonly ITimeProvider _timeProvider;

    public SendGoodMorningJob(
        ITelegramBotClient botClient,
        IStringLocalizer<CollectMoodRecordsJob> localizer,
        IUserService userService,
        ITimeProvider timeProvider)
    {
        _botClient = botClient;
        _localizer = localizer;
        _userService = userService;
        _timeProvider = timeProvider;
    }

    public override async Task Execute(IJobExecutionContext context)
    {
        var time = _timeProvider.Now.GetTime();

        var users = (await _userService.GetAllAsync(context.CancellationToken))
            .Where(u => u.NotificationsEnabled
                && time.Add(u.Gmt) == _morning)
            .ToList();

        if (users.IsNullOrEmpty())
        {
            return;
        }

        var notifications = new List<Task>(users.Count);

        foreach (var user in users)
        {
            using var _ = _localizer.UsingCulture(user.Culture);

            notifications.Add(
                _botClient.SendMessage(
                    chatId: user.ChatId,
                    text: _localizer["Jobs:Morning:SendGoodMorning"],
                    cancellationToken: context.CancellationToken
                )
            );
        }

        await Task.WhenAll(notifications);
    }
}
