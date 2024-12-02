using Eclipse.Application.Contracts.Reminders;
using Eclipse.Application.Reminders;
using Eclipse.Common.Results;
using Eclipse.Domain.Reminders;
using Eclipse.Domain.Users.Events;
using Eclipse.Localization.Culture;
using Eclipse.Localization.Extensions;

using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

using NSubstitute;
using NSubstitute.Extensions;

using Quartz;

using Telegram.Bot;
using Telegram.Bot.Requests;

using Xunit;

namespace Eclipse.Application.Tests.Reminders;

public sealed class SendReminderJobTests
{
    private readonly ITelegramBotClient _client;

    private readonly IStringLocalizer<SendReminderJob> _localizer;

    private readonly IReminderService _reminderService;

    private readonly ILogger<SendReminderJob> _logger;

    private readonly ICurrentCulture _currentCulture;

    private readonly SendReminderJob _sut;

    public SendReminderJobTests()
    {
        _client = Substitute.For<ITelegramBotClient>();
        _localizer = Substitute.For<IStringLocalizer<SendReminderJob>>();
        _reminderService = Substitute.For<IReminderService>();
        _logger = Substitute.For<ILogger<SendReminderJob>>();
        _currentCulture = Substitute.For<ICurrentCulture>();

        _sut = new SendReminderJob(_client, _localizer, _reminderService, _logger, _currentCulture);
    }

    [Theory]
    [InlineData("test", "en", 1)]
    public async Task Execute_WhenTriggered_ThenSendsReminder_AndUnschedulesJob(string text, string culture, long chatId)
    {
        var userId = Guid.NewGuid();
        var reminderId = Guid.NewGuid();

        var data = new ReminderAddedDomainEvent(reminderId, userId, new TimeSpan(), new TimeOnly(), text, culture, chatId);

        var map = new JobDataMap((IDictionary<string, object>)new Dictionary<string, object>
        {
            ["data"] = JsonConvert.SerializeObject(data)
        });

        var context = Substitute.For<IJobExecutionContext>();
        
        context.MergedJobDataMap.Returns(map);

        var scheduler = Substitute.For<IScheduler>();
        context.Scheduler.Returns(scheduler);

        _localizer["Jobs:SendReminders:Message"]
            .Returns(new LocalizedString("Jobs:SendReminders:Message", "Reminder:"));

        _reminderService.DeleteAsync(userId, reminderId).Returns(Result.Success());

        await _sut.Execute(context);

        _currentCulture.Received().UsingCulture(culture);
        _localizer.Received().UseCurrentCulture(_currentCulture);

        await _client.Received().SendRequest(
            Arg.Is<SendMessageRequest>(request => request.ChatId == chatId
                && request.Text == $"Reminder:\n\r\n\r{text}"
            )
        );

        await _reminderService.Received().DeleteAsync(userId, reminderId);
        await scheduler.Received().UnscheduleJob(Arg.Any<TriggerKey>());
    }
}
