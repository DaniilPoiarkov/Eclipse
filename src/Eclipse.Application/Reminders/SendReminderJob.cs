using Eclipse.Application.Contracts.Reminders;
using Eclipse.Localization.Culture;
using Eclipse.Localization.Extensions;

using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

using Quartz;

using Telegram.Bot;

namespace Eclipse.Application.Reminders;

internal sealed class SendReminderJob : IJob
{
    private readonly ITelegramBotClient _client;

    private readonly IStringLocalizer<SendReminderJob> _localizer;

    private readonly IReminderService _reminderService;

    private readonly ILogger<SendReminderJob> _logger;

    private readonly ICurrentCulture _currentCulture;

    public SendReminderJob(
        ITelegramBotClient client,
        IStringLocalizer<SendReminderJob> localizer,
        IReminderService reminderService,
        ILogger<SendReminderJob> logger,
        ICurrentCulture currentCulture)
    {
        _client = client;
        _localizer = localizer;
        _reminderService = reminderService;
        _logger = logger;
        _currentCulture = currentCulture;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var data = context.MergedJobDataMap.GetString("data");

        if (data.IsNullOrEmpty())
        {
            _logger.LogError("Cannot deserialize event with data {Data}", "{null}");
            return;
        }

        var reminder = JsonConvert.DeserializeObject<SendReminderJobData>(data);

        if (reminder is null)
        {
            _logger.LogError("Cannot deserialize event with data {Data}", data);
            return;
        }

        using var _ = _currentCulture.UsingCulture(reminder.Culture);
        _localizer.UseCurrentCulture(_currentCulture);

        var message = $"{_localizer["Jobs:SendReminders:Message"]}\n\r\n\r{reminder.Text}";

        await _client.SendMessage(reminder.ChatId, message, cancellationToken: context.CancellationToken);

        var result = await _reminderService.DeleteAsync(reminder.UserId, reminder.ReminderId, context.CancellationToken);

        if (!result.IsSuccess)
        {
            _logger.LogError("Failed to remove reminder with id {Id} for user {UserId}. Error: {Error}", reminder.ReminderId, reminder.UserId, result.Error);
        }

        await context.Scheduler.UnscheduleJob(context.Trigger.Key, context.CancellationToken);
    }
}
