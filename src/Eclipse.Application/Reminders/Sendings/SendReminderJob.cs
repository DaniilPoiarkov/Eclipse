using Eclipse.Application.Contracts.Reminders;
using Eclipse.Localization.Culture;

using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

using Quartz;

using Telegram.Bot;

namespace Eclipse.Application.Reminders.Sendings;

internal sealed class SendReminderJob : IJob
{
    private readonly ILogger<SendReminderJob> _logger;

    private readonly IReminderSender _reminderSender;

    public SendReminderJob(
        ILogger<SendReminderJob> logger,
        IReminderSender reminderSender)
    {
        _logger = logger;
        _reminderSender = reminderSender;
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

        await _reminderSender.Send(
            new ReminderArguments(
                reminder.ReminderId,
                reminder.UserId,
                reminder.RelatedItemId,
                reminder.Text,
                reminder.Culture,
                reminder.ChatId
            ),
            context.CancellationToken
        );
    }
}
