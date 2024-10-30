using Eclipse.Application.Contracts.Users;
using Eclipse.Localization.Culture;
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

    private readonly ICurrentCulture _currentCulture;

    private readonly IUserService _userService;

    public SendGoodMorningJob(
        ITelegramBotClient botClient,
        IStringLocalizer<CollectMoodRecordsJob> localizer,
        ICurrentCulture currentCulture,
        IUserService userService)
    {
        _botClient = botClient;
        _localizer = localizer;
        _currentCulture = currentCulture;
        _userService = userService;
    }

    public override async Task Execute(IJobExecutionContext context)
    {
        var time = DateTime.UtcNow.GetTime();

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
            using var _ = _currentCulture.UsingCulture(user.Culture);
            _localizer.UseCurrentCulture(_currentCulture);

            notifications.Add(
                _botClient.SendTextMessageAsync(
                    chatId: user.ChatId,
                    text: _localizer["Jobs:Morning:SendGoodMorning"],
                    cancellationToken: context.CancellationToken
                )
            );
        }

        await Task.WhenAll(notifications);
    }
}
