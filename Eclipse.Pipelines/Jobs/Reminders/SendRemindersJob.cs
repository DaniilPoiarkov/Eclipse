using Eclipse.Application.Contracts.Localizations;
using Eclipse.Application.Contracts.Reminders;

using Quartz;

using Telegram.Bot;

namespace Eclipse.Pipelines.Jobs.Reminders;

public class SendRemindersJob : IJob
{
    private readonly IReminderService _reminderService;

    private readonly ITelegramBotClient _botClient;

    private readonly IEclipseLocalizer _localizer;

    public SendRemindersJob(IReminderService reminderService, ITelegramBotClient botClient, IEclipseLocalizer localizer)
    {
        _reminderService = reminderService;
        _botClient = botClient;
        _localizer = localizer;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var reminders = await _reminderService.GetForSpecifiedTimeAsync(TimeOnly.FromDateTime(DateTime.UtcNow), context.CancellationToken);

        if (reminders.Count == 0)
        {
            return;
        }

        var messageSendings = new List<Task>(reminders.Count);

        foreach (var reminder in reminders)
        {
            _localizer.CheckCulture(reminder.User.ChatId);

            var message = $"{_localizer["Jobs:SendReminders:Message"]}\n\r\n\r{reminder.Text}";

            var sending = _botClient.SendTextMessageAsync(reminder.User.ChatId, message, cancellationToken: context.CancellationToken);

            messageSendings.Add(sending);
        }

        await Task.WhenAll(messageSendings);
    }
}
