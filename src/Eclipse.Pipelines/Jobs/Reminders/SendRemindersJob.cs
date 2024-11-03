using Eclipse.Application.Contracts.Reminders;
using Eclipse.Application.Contracts.Users;
using Eclipse.Common.Clock;
using Eclipse.Localization.Culture;
using Eclipse.Localization.Extensions;

using Microsoft.Extensions.Localization;

using Quartz;

using Telegram.Bot;

namespace Eclipse.Pipelines.Jobs.Reminders;

internal sealed class SendRemindersJob : EclipseJobBase
{
    private readonly ITelegramBotClient _botClient;

    private readonly IStringLocalizer<SendRemindersJob> _localizer;

    private readonly IReminderService _reminderService;

    private readonly ICurrentCulture _currentCulture;

    private readonly IUserService _userService;

    private readonly ITimeProvider _timeProvider;

    public SendRemindersJob(
        ITelegramBotClient botClient,
        IStringLocalizer<SendRemindersJob> localizer,
        IReminderService reminderService,
        ICurrentCulture currentCulture,
        IUserService userService,
        ITimeProvider timeProvider)
    {
        _botClient = botClient;
        _localizer = localizer;
        _reminderService = reminderService;
        _currentCulture = currentCulture;
        _userService = userService;
        _timeProvider = timeProvider;
    }

    public override async Task Execute(IJobExecutionContext context)
    {
        var time = _timeProvider.Now.GetTime();

        var specification = new ReminderDtoNotifyAtSpecification(time);

        var users = (await _userService.GetAllAsync(context.CancellationToken))
            .Where(user => user.Reminders.Any(specification))
            .ToList();

        if (users.IsNullOrEmpty())
        {
            return;
        }

        var operations = new List<Task>(users.Count);

        foreach (var user in users)
        {
            using var _ = _currentCulture.UsingCulture(user.Culture);
            _localizer.UseCurrentCulture(_currentCulture);

            var messageSendings = user.Reminders
                .Where(specification)
                .Select(reminder => $"{_localizer["Jobs:SendReminders:Message"]}\n\r\n\r{reminder.Text}")
                .Select(message => _botClient.SendTextMessageAsync(user.ChatId, message, cancellationToken: context.CancellationToken));

            operations.AddRange(messageSendings);

            await _reminderService.RemoveForTimeAsync(user.Id, time, context.CancellationToken);
        }

        await Task.WhenAll(operations);
    }
}
