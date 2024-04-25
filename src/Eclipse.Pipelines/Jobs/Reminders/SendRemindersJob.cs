using Eclipse.Application.Contracts.Localizations;
using Eclipse.Application.Contracts.Reminders;
using Eclipse.Pipelines.Users;

using Quartz;

using Telegram.Bot;

namespace Eclipse.Pipelines.Jobs.Reminders;

internal sealed class SendRemindersJob : EclipseJobBase
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
        var time = DateTime.UtcNow.GetTime();

        var specification = new ReminderDtoNotifyAtSpecification(time);

        var users = (await _userStore.GetCachedUsersAsync(context.CancellationToken))
            .Where(u => u.Reminders.Any(specification))
            .ToList();

        if (users.IsNullOrEmpty())
        {
            return;
        }

        var operations = new List<Task>(users.Count);

        foreach (var user in users)
        {
            await _localizer.ResetCultureForUserWithChatIdAsync(user.ChatId, context.CancellationToken);

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
