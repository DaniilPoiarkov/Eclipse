using Eclipse.Application.Contracts.IdentityUsers;
using Eclipse.Application.Contracts.Localizations;
using Eclipse.Application.Contracts.Reminders;
using Eclipse.Domain.Reminders;

using Quartz;

using Telegram.Bot;

namespace Eclipse.Pipelines.Jobs.Reminders;

internal class SendRemindersJob : EclipseJobBase
{
    private readonly ITelegramBotClient _botClient;

    private readonly IEclipseLocalizer _localizer;

    private readonly IIdentityUserStore _identityUserStore;

    private readonly ReminderManager _reminderManager;

    public SendRemindersJob(ITelegramBotClient botClient, IEclipseLocalizer localizer, IIdentityUserStore identityUserStore, ReminderManager reminderManager)
    {
        _botClient = botClient;
        _localizer = localizer;
        _identityUserStore = identityUserStore;
        _reminderManager = reminderManager;
    }

    public override async Task Execute(IJobExecutionContext context)
    {
        var utc = DateTime.UtcNow;
        var time = new TimeOnly(utc.Hour, utc.Minute);

        var dtoSpecification = new ReminderDtoNotifyAtSpecification(time);

        var users = _identityUserStore.GetCachedUsers()
            .Where(u => u.Reminders.Any(dtoSpecification))
            .ToList();

        if (users.Count == 0)
        {
            return;
        }

        var operations = new List<Task>(users.Count);

        foreach (var user in users)
        {
            _localizer.CheckCulture(user.ChatId);

            var reminders = user.Reminders.Where(dtoSpecification);

            var messageSendings = reminders
                .Select(reminder => $"{_localizer["Jobs:SendReminders:Message"]}\n\r\n\r{reminder.Text}")
                .Select(message => _botClient.SendTextMessageAsync(user.ChatId, message, cancellationToken: context.CancellationToken));

            operations.AddRange(messageSendings);

            await _reminderManager.RemoveRemindersForTime(user.Id, time, context.CancellationToken);
        }

        await Task.WhenAll(operations);
    }
}
