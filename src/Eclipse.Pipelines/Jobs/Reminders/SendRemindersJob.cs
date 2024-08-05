using Eclipse.Application.Contracts.Reminders;
using Eclipse.Localization.Culture;
using Eclipse.Localization.Extensions;
using Eclipse.Pipelines.Users;

using Microsoft.Extensions.Localization;

using Quartz;

using Telegram.Bot;

namespace Eclipse.Pipelines.Jobs.Reminders;

internal sealed class SendRemindersJob : EclipseJobBase
{
    private readonly ITelegramBotClient _botClient;

    private readonly IStringLocalizer<SendRemindersJob> _localizer;

    private readonly IUserStore _userStore;

    private readonly IReminderService _reminderService;

    private readonly ICurrentCulture _currentCulture;

    public SendRemindersJob(
        ITelegramBotClient botClient,
        IStringLocalizer<SendRemindersJob> localizer,
        IUserStore userStore,
        IReminderService reminderService,
        ICurrentCulture currentCulture)
    {
        _botClient = botClient;
        _localizer = localizer;
        _userStore = userStore;
        _reminderService = reminderService;
        _currentCulture = currentCulture;
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
