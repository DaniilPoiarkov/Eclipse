using Eclipse.Application.Contracts.Localizations;
using Eclipse.Application.Contracts.Reminders;
using Eclipse.Pipelines.Users;
using Quartz;

using Telegram.Bot;

namespace Eclipse.Pipelines.Jobs.Reminders;

internal class SendRemindersJob : EclipseJobBase
{
    private readonly ITelegramBotClient _botClient;

    private readonly IEclipseLocalizer _localizer;

    private readonly IUserStore _userStore;

    private readonly IReminderService _reminderService;

    public SendRemindersJob(ITelegramBotClient botClient, IEclipseLocalizer localizer, IUserStore userStore, IReminderService reminderService)
    {
        _botClient = botClient;
        _localizer = localizer;
        _userStore = userStore;
        _reminderService = reminderService;
    }

    public override async Task Execute(IJobExecutionContext context)
    {
        var utc = DateTime.UtcNow;
        var time = new TimeOnly(utc.Hour, utc.Minute);

        var specification = new ReminderDtoNotifyAtSpecification(time);

        var users = _userStore.GetCachedUsers()
            .Where(u => u.Reminders.Any(specification))
            .ToList();

        if (users.Count == 0)
        {
            return;
        }

        var operations = new List<Task>(users.Count);

        foreach (var user in users)
        {
            _localizer.CheckCulture(user.ChatId);

            var messageSendings = user.Reminders
                .Where(specification)
                .Select(reminder => $"{_localizer["Jobs:SendReminders:Message"]}\n\r\n\r{reminder.Text}")
                .Select(message => _botClient.SendTextMessageAsync(user.ChatId, message, cancellationToken: context.CancellationToken));

            operations.AddRange(messageSendings);

            await _reminderService.RemoveRemindersForTime(user.Id, time, context.CancellationToken);
        }

        await Task.WhenAll(operations);
    }
}
