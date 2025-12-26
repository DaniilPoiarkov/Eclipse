using Eclipse.Application.Reminders.Sendings;
using Eclipse.Domain.Users.Events;

using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

using NSubstitute;

using Quartz;

using Xunit;

namespace Eclipse.Application.Tests.Reminders;

public sealed class SendReminderJobTests
{
    private readonly IReminderSender _reminderSender;

    private readonly ILogger<SendReminderJob> _logger;

    private readonly SendReminderJob _sut;

    public SendReminderJobTests()
    {
        _logger = Substitute.For<ILogger<SendReminderJob>>();
        _reminderSender = Substitute.For<IReminderSender>();

        _sut = new SendReminderJob(_logger, _reminderSender);
    }

    [Theory]
    [InlineData("test", "en", 1)]
    public async Task Execute_WhenTriggered_ThenSendsReminder_AndUnschedulesJob(string text, string culture, long chatId)
    {
        var userId = Guid.NewGuid();
        var reminderId = Guid.NewGuid();

        var data = new ReminderAddedDomainEvent(reminderId, userId, Guid.NewGuid(), new TimeSpan(), new TimeOnly(), text, culture, chatId);

        var map = new JobDataMap((IDictionary<string, object>)new Dictionary<string, object>
        {
            ["data"] = JsonConvert.SerializeObject(data)
        });

        var context = Substitute.For<IJobExecutionContext>();

        context.MergedJobDataMap.Returns(map);

        var scheduler = Substitute.For<IScheduler>();
        context.Scheduler.Returns(scheduler);

        await _sut.Execute(context);

        await _reminderSender.Received().Send(
            Arg.Is<ReminderArguments>(a => a.ReminderId == data.ReminderId
                && a.UserId == data.UserId
                && a.RelatedItemId == data.RelatedItemId
                && a.Culture == data.Culture
                && a.Text == data.Text
                && a.ChatId == data.ChatId
            )
        );
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("\"\"")]
    public async Task Execute_WhenDataInvalid_ThenLogsError(string? data)
    {
        var context = Substitute.For<IJobExecutionContext>();

        var map = new JobDataMap((IDictionary<string, object>)new Dictionary<string, object>
        {
            ["data"] = data!
        });

        context.MergedJobDataMap.Returns(map);

        await _sut.Execute(context);

        _logger.Received().Log(
            LogLevel.Error,
            Arg.Any<EventId>(),
            Arg.Any<object>(),
            Arg.Any<Exception>(),
            Arg.Any<Func<object, Exception?, string>>()
        );
    }
}
