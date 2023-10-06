using Eclipse.Application.Contracts.Localizations;
using Eclipse.Domain.IdentityUsers;

using Quartz;

using Telegram.Bot;

namespace Eclipse.Pipelines.Jobs.Reminders;

internal class SendRemindersJob : EclipseJobBase
{
    private readonly ITelegramBotClient _botClient;

    private readonly IEclipseLocalizer _localizer;

    private readonly IdentityUserManager _userManager;

    public SendRemindersJob(ITelegramBotClient botClient, IEclipseLocalizer localizer, IdentityUserManager reminderManager)
    {
        _botClient = botClient;
        _localizer = localizer;
        _userManager = reminderManager;
    }

    public override async Task Execute(IJobExecutionContext context)
    {
        var utc = DateTime.UtcNow;
        var time = new TimeOnly(utc.Hour, utc.Minute);

        var users = await _userManager.GetUsersWithRemindersInSpecifiedTime(time, context.CancellationToken);

        if (users.Count == 0)
        {
            return;
        }

        var operations = new List<Task>(users.Count);

        foreach (var user in users)
        {
            _localizer.CheckCulture(user.ChatId);

            var reminders = user.GetRemindersForTime(time);

            var messageSendings = reminders
                .Select(reminder => $"{_localizer["Jobs:SendReminders:Message"]}\n\r\n\r{reminder.Text}")
                .Select(message => _botClient.SendTextMessageAsync(user.ChatId, message, cancellationToken: context.CancellationToken));

            operations.AddRange(messageSendings);

            user.RemoveReminders(reminders);

            await _userManager.UpdateAsync(user, context.CancellationToken);
        }

        await Task.WhenAll(operations);
    }
}
